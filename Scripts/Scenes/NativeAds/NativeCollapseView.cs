#if ADMOB_NATIVE_ADS
namespace TheOneStudio.UITemplate.UITemplate.Scenes.NativeAds
{
    using System;
    using Core.AdsServices.Native;
    using GameFoundation.DI;
    using GameFoundation.Signals;
    using ServiceImplementation.Configs.Ads;
    using TheOne.Logging;
    using TheOneStudio.UITemplate.Scripts.Signals;
    using TheOneStudio.UITemplate.UITemplate.Scripts.ThirdPartyServices;
    using UnityEngine;
    using UnityEngine.UI;
    using ILogger = TheOne.Logging.ILogger;

    public class NativeCollapseView : MonoBehaviour
    {
        [SerializeField] private GameObject    view;
        [SerializeField] private NativeAdsView nativeAdsView;
        [SerializeField] private Button        btnClose;

        private SignalBus                  signalBus;
        private UITemplateAdServiceWrapper adServiceWrapper;
        private INativeAdsService          nativeAdsService;
        private IAdMobNativeAdsService     adMobNativeAdsService;
        private ILogger                    logger;
        private bool                       isShow;

        private void Awake()
        {
            var container = this.GetCurrentContainer();
            this.signalBus             = container.Resolve<SignalBus>();
            this.adServiceWrapper      = container.Resolve<UITemplateAdServiceWrapper>();
            this.nativeAdsService      = container.Resolve<INativeAdsService>();
            this.adMobNativeAdsService = (IAdMobNativeAdsService)this.nativeAdsService;
            this.logger                = container.Resolve<ILoggerManager>().GetLogger(this);

            this.view.SetActive(false);
            this.btnClose.onClick.AddListener(this.OnHide);

            this.signalBus.Subscribe<ShowNativeCollapseSignal>(this.OnShowNativeCollapseSignal);
        }

        private void OnDestroy()
        {
            this.signalBus.Unsubscribe<ShowNativeCollapseSignal>(this.OnShowNativeCollapseSignal);
        }

        private void OnShowNativeCollapseSignal(ShowNativeCollapseSignal signal)
        {
            if (signal.IsShow)
            {
                this.OnShow();
            }
            else
            {
                this.OnHide();
            }
        }

        private void OnShow()
        {
            this.isShow = this.adMobNativeAdsService.IsNativeAdsReady(this.nativeAdsView.Placement);
            this.logger.Info($"Native ads ready: {this.isShow}, placement: {this.nativeAdsView.Placement}");
            if (this.isShow)
            {
                this.view.SetActive(true);
                this.nativeAdsView.Init(this.nativeAdsService);
            }
        }

        private void OnHide()
        {
            if (!this.isShow) return;
            this.isShow = false;
            this.view.SetActive(false);
            this.nativeAdsView.Release();
            this.adServiceWrapper.InternalCloseNativeCollapse();
        }
    }
}
#endif