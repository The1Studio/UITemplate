namespace TheOneStudio.UITemplate.UITemplate.Scenes.BottomBarNavigator
{
    using System.Collections.Generic;
    using System.Linq;
    using DG.Tweening;
    using GameFoundation.Scripts.UIModule.ScreenFlow.Signals;
    using TheOneStudio.UITemplate.UITemplate.Scripts.ThirdPartyServices;
    using TheOneStudio.UITemplate.UITemplate.Signals;
    using UnityEngine;
    using Zenject;

    public class UITemplateBottomNavigatorBarView : MonoBehaviour
    {
        #region inject

        private SignalBus                  signalBus;
        private UITemplateAdServiceWrapper uiTemplateAdServiceWrapper;

        #endregion

        public Transform buttonParent;

        private List<BottomBarNavigatorTabButtonView> Buttons;

        private int  CurrentActiveIndex { get; set; } = -1;
        private bool IsShowingBar              = true;
        private bool IsFirstTimeOpenDefaultTab = true;

        [Inject]
        public void Constructor(SignalBus signalBus, UITemplateAdServiceWrapper uiTemplateAdServiceWrapper)
        {
            this.signalBus                  = signalBus;
            this.uiTemplateAdServiceWrapper = uiTemplateAdServiceWrapper;

            this.Init();
        }

        private void Init()
        {
            this.signalBus.Subscribe<OnRemoveAdsSucceedSignal>(this.OnRemoveAdsHandler);
            this.signalBus.Subscribe<ScreenShowSignal>(this.OnScreenShowSignalHandler);
            this.signalBus.Subscribe<ScreenCloseSignal>(this.OnScreenCloseSignalHandler);
        }

        private void OnScreenCloseSignalHandler(ScreenCloseSignal obj) { this.OnChangeFocusScreen(); }

        private void OnScreenShowSignalHandler(ScreenShowSignal obj) { this.OnChangeFocusScreen(); }

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
                return;
            }

            if (!this.IsShowingBar) return;
            this.IsShowingBar = false;
            rectTransform.DOKill();
            rectTransform.DOSizeDelta(Vector2.up * this.HiddenHeight, animationDuration).SetEase(Ease.InBack).SetUpdate(true);
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
                bottomBarNavigatorTabButtonView.Button.onClick.AddListener(() => this.OnClickBottomBarButton(index1));
            }

            this.OnClickBottomBarButton(this.DefaultActiveIndex);
        }

        private void OnClickBottomBarButton(int index)
        {
            if (this.CurrentActiveIndex == index) return;
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

        protected virtual void OnCLickButton(int index) { }

        protected virtual int DefaultActiveIndex => 0;
        protected virtual int HasBannerHeight    => 350;
        protected virtual int NoBannerHeight     => 250;
        protected virtual int HiddenHeight       => -100;

        protected virtual bool IsShouldShowBar() => true;
        private           int  Height            => this.uiTemplateAdServiceWrapper.IsRemovedAds ? this.NoBannerHeight : this.HasBannerHeight;
    }
}