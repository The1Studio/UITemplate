namespace TheOneStudio.UITemplate.UITemplate.Scenes.BottomBarNavigator
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using DG.Tweening;
    using GameFoundation.Scripts.UIModule.ScreenFlow.BaseScreen.Presenter;
    using GameFoundation.Scripts.UIModule.ScreenFlow.Managers;
    using GameFoundation.Scripts.UIModule.ScreenFlow.Signals;
    using TheOneStudio.UITemplate.UITemplate.Scenes.Main;
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

        #endregion

        public Transform buttonParent;

        protected List<BottomBarNavigatorTabButtonView> Buttons;

        private int  CurrentActiveIndex { get; set; } = -1;
        private bool IsShowingBar              = true;
        private bool IsFirstTimeOpenDefaultTab = true;

        private           Dictionary<Type, int> allcurrentScreen = new Dictionary<Type, int>();
        protected virtual Dictionary<Type, int> AllcurrentScreen => this.allcurrentScreen;

        [Inject]
        public void Constructor(SignalBus signalBus, UITemplateAdServiceWrapper uiTemplateAdServiceWrapper, IScreenManager screenManager)
        {
            this.signalBus                  = signalBus;
            this.uiTemplateAdServiceWrapper = uiTemplateAdServiceWrapper;
            this.screenManager              = screenManager;
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
                rectTransform.DOAnchorMax(new Vector2(1, this.Anchor), animationDuration).SetEase(Ease.OutBack).SetUpdate(true);
                this.OnDoShowBar();
                return;
            }

            if (!this.IsShowingBar) return;
            this.IsShowingBar = false;
            rectTransform.DOKill();
            rectTransform.DOAnchorMax(new Vector2(1, this.HiddenAnchor), animationDuration).SetEase(Ease.InBack).SetUpdate(true);
        }

        protected virtual void OnDoShowBar()
        {
            
        }

        protected virtual void OnRemoveAdsHandler()
        {
            var rectTransform = this.transform as RectTransform;
            rectTransform.DOKill();
            rectTransform.DOAnchorMax(new Vector2(1, this.NoBannerAnchor), 0.3f);
        }

        private void Awake()
        {
            ((RectTransform)this.transform).anchorMax = new Vector2(1, this.Anchor);
            this.Buttons                              = this.buttonParent.GetComponentsInChildren<BottomBarNavigatorTabButtonView>().ToList();

            for (var index = 0; index < this.Buttons.Count; index++)
            {
                var bottomBarNavigatorTabButtonView = this.Buttons[index];
                bottomBarNavigatorTabButtonView.Init();
                var index1 = index;
                bottomBarNavigatorTabButtonView.Button.onClick.AddListener(() => this.OnClickBottomBarButton(index1));
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
        
        protected virtual float HasBannerAnchor => 0.15f;
        protected virtual float NoBannerAnchor  => 0.1f;
        protected virtual float HiddenAnchor    => -this.NoBannerAnchor;

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

        private float Anchor => this.uiTemplateAdServiceWrapper.IsRemovedAds ? this.NoBannerAnchor : this.HasBannerAnchor;
    }
}