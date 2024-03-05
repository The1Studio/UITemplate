namespace TheOneStudio.UITemplate.UITemplate.Scenes.BottomBarNavigator
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using DG.Tweening;
    using GameFoundation.Scripts.UIModule.ScreenFlow.Managers;
    using GameFoundation.Scripts.UIModule.ScreenFlow.Signals;
    using GameFoundation.Scripts.Utilities;
    using TheOneStudio.UITemplate.UITemplate.Configs.GameEvents;
    using TheOneStudio.UITemplate.UITemplate.Extension;
    using TheOneStudio.UITemplate.UITemplate.Interfaces;
    using TheOneStudio.UITemplate.UITemplate.Scripts.ThirdPartyServices;
    using TheOneStudio.UITemplate.UITemplate.Signals;
    using UnityEngine;
    using Zenject;

    public abstract class UITemplateBottomNavigatorBarView : MonoBehaviour
    {
        #region inject

        private SignalBus                  signalBus;
        private UITemplateAdServiceWrapper uiTemplateAdServiceWrapper;
        private IScreenManager             screenManager;
        private IVibrationService          vibrationService;
        private GameFeaturesSetting        gameFeaturesSetting;
        private IAudioService              audioService;

        #endregion

        public Transform buttonParent;

        protected List<BottomBarNavigatorTabButtonView> Buttons;

        private int  CurrentActiveIndex { get; set; } = -1;
        private bool IsShowingBar              = true;
        private bool IsFirstTimeOpenDefaultTab = true;

        private           Dictionary<Type, int> allcurrentScreen = new Dictionary<Type, int>();
        protected virtual Dictionary<Type, int> AllcurrentScreen => this.allcurrentScreen;

        [Inject]
        public void Constructor(SignalBus signalBus, UITemplateAdServiceWrapper uiTemplateAdServiceWrapper, IScreenManager screenManager, IVibrationService vibrationService, GameFeaturesSetting gameFeaturesSetting, IAudioService audioService)
        {
            this.signalBus                  = signalBus;
            this.uiTemplateAdServiceWrapper = uiTemplateAdServiceWrapper;
            this.screenManager              = screenManager;
            this.vibrationService           = vibrationService;
            this.gameFeaturesSetting        = gameFeaturesSetting;
            this.audioService              = audioService;
        }

        protected virtual void Init()
        {
            this.signalBus.Subscribe<OnRemoveAdsSucceedSignal>(this.OnRemoveAdsHandler);
            this.signalBus.Subscribe<ScreenShowSignal>(this.OnScreenShowSignalHandler);
            this.signalBus.Subscribe<ScreenCloseSignal>(this.OnScreenCloseSignalHandler);
            this.RegisterScreens();
        }
        
        /// <summary>
        /// Example:
        /// this.AllcurrentScreen.Add(typeof(LeaderboardScreenPresenter), 0);
        /// this.AllcurrentScreen.Add(typeof(CollectionScreenPresenter), 1);
        /// this.AllcurrentScreen.Add(typeof(HomeScreenPresenter), 2);
        /// this.AllcurrentScreen.Add(typeof(GachaScreenPresenter), 3);
        /// this.AllcurrentScreen.Add(typeof(ShopPackScreenPresenter), 4);
        /// </summary>
        protected abstract void RegisterScreens();

        private void OnScreenCloseSignalHandler(ScreenCloseSignal obj) { this.OnChangeFocusScreen(); }

        private void OnScreenShowSignalHandler(ScreenShowSignal obj)
        {
            if (this.AllcurrentScreen.TryGetValue(this.screenManager.CurrentActiveScreen.Value.GetType(), out var currentScreen))
            {
                this.CurrentActiveIndex = currentScreen;
            }

            this.OnChangeFocusScreen();
            if (this.AllcurrentScreen.ContainsKey(this.screenManager.CurrentActiveScreen.Value.GetType()))
            {
                this.OnClickBottomBarButton(this.CurrentActiveIndex);
            }
        }

        private void OnChangeFocusScreen()
        {
            var rectTransform     = this.transform as RectTransform;
            var animationDuration = 0.5f;
            if (this.IsShouldShowBar())
            {
                if (this.IsShowingBar) return;
                this.IsShowingBar = true;
                rectTransform.DOKill();
                rectTransform.DOSizeDelta(Vector2.up * this.Height, animationDuration).SetEase(Ease.OutBack).SetUpdate(true);
                this.OnDoShowBar();
                return;
            }

            if (!this.IsShowingBar) return;
            this.IsShowingBar = false;
            rectTransform.DOKill();
            rectTransform.DOSizeDelta(Vector2.up * this.HiddenHeight, animationDuration).SetEase(Ease.InBack).SetUpdate(true);
        }

        protected virtual void OnDoShowBar()
        {
            
        }

        protected virtual void OnRemoveAdsHandler()
        {
            var rectTransform = this.transform as RectTransform;
            rectTransform.DOKill();
            rectTransform.DOSizeDelta(Vector2.up * this.NoBannerHeight, 0.3f);
        }

        private void Awake()
        {
            ((RectTransform)this.transform).sizeDelta = Vector2.up * this.Height;
            this.Buttons                              = this.buttonParent.GetComponentsInChildren<BottomBarNavigatorTabButtonView>().ToList();

            for (var index = 0; index < this.Buttons.Count; index++)
            {
                var bottomBarNavigatorTabButtonView = this.Buttons[index];
                bottomBarNavigatorTabButtonView.Init();
                var index1 = index;
                bottomBarNavigatorTabButtonView.Button.onClick.AddListener(() =>
                {
                    this.OnClickBottomBarButton(index1);
                    // this.vibrationService.PlayPresetType(this.gameFeaturesSetting.vibrationPresetType);
                    if (!this.gameFeaturesSetting.clickButtonSound.IsNullOrEmpty())
                    {
                        this.audioService.PlaySound(this.gameFeaturesSetting.clickButtonSound);
                    }
                });
            }

            this.OnClickBottomBarButton(this.DefaultActiveIndex);

            this.Init();
        }

        protected void OnClickBottomBarButton(int index)
        {
            this.CurrentActiveIndex = index;

            //Update bar view
            var bottomBarNavigatorTabButtonView = this.Buttons[index];
            bottomBarNavigatorTabButtonView.SetActive(true);
            foreach (var otherBottomBarNavigatorTabButtonView in this.Buttons)
            {
                if (otherBottomBarNavigatorTabButtonView == bottomBarNavigatorTabButtonView) continue;

                otherBottomBarNavigatorTabButtonView.SetActive(false);
            }

            //Do change tab or open screen
            if (!this.IsFirstTimeOpenDefaultTab)
            {
                this.OnCLickButton(index);
            }
            else
            {
                this.IsFirstTimeOpenDefaultTab = false;
            }
        }

        protected abstract void OnCLickButton(int index);

        protected abstract int DefaultActiveIndex { get; }
        protected virtual  int HasBannerHeight    => 380;
        protected virtual  int NoBannerHeight     => 250;
        protected virtual  int HiddenHeight       => -200;

        /// <summary> example
        /// return this.screenManager.CurrentActiveScreen.Value
        /// is CampaignScreenPresenter
        ///    or UpgradeScreenPresenter
        ///    or HomeSimpleScreenPresenter
        ///    or CollectionScreenPresenter
        ///    or ShopPackScreenPresenter;
        /// </summary>
        /// <returns></returns>
        protected abstract bool IsShouldShowBar();

        private int Height => this.uiTemplateAdServiceWrapper.IsRemovedAds ? this.NoBannerHeight : this.HasBannerHeight;
    }
}