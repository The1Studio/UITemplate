namespace TheOneStudio.UITemplate.UITemplate.Scenes.BottomBarNavigator
{
    using System.Collections.Generic;
    using System.Linq;
    using DG.Tweening;
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
        private int                                   CurrentActiveIndex { get; set; } = -1;

        [Inject]
        public void Constructor(SignalBus signalBus, UITemplateAdServiceWrapper uiTemplateAdServiceWrapper)
        {
            this.signalBus                  = signalBus;
            this.uiTemplateAdServiceWrapper = uiTemplateAdServiceWrapper;

            this.Init();
        }

        private void Init() { this.signalBus.Subscribe<OnRemoveAdsSucceedSignal>(this.OnRemoveAdsHandler); }

        protected virtual void OnRemoveAdsHandler() { (this.transform as RectTransform).DOSizeDelta(Vector2.up * this.NoBannerHeight, 0.5f); }

        private void Awake()
        {
            ((RectTransform)this.transform).sizeDelta = Vector2.up * (this.uiTemplateAdServiceWrapper.IsRemovedAds ? this.NoBannerHeight : this.HasBannerHeight);
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
            Debug.Log($"On Click: {index}");
            if (this.CurrentActiveIndex == index) return;
            this.CurrentActiveIndex = index;

            this.OnCLickButton(index);
            var bottomBarNavigatorTabButtonView = this.Buttons[index];
            bottomBarNavigatorTabButtonView.SetActive(true);
            foreach (var otherBottomBarNavigatorTabButtonView in this.Buttons)
            {
                if (otherBottomBarNavigatorTabButtonView == bottomBarNavigatorTabButtonView) continue;

                otherBottomBarNavigatorTabButtonView.SetActive(false);
            }
        }

        protected virtual void OnCLickButton(int index) { }

        protected virtual int DefaultActiveIndex => 0;
        protected virtual int HasBannerHeight    => 350;
        protected virtual int NoBannerHeight     => 250;
    }
}