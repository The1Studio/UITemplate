#if ADMOB_NATIVE_ADS
namespace TheOneStudio.UITemplate.UITemplate.Scenes.NativeAds
{
    using System;
    using System.Threading;
    using Core.AdsServices.Native;
    using Cysharp.Threading.Tasks;
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
        [SerializeField] private BoxCollider   boxCollider;

        private SignalBus                  signalBus;
        private UITemplateAdServiceWrapper adServiceWrapper;
        private INativeAdsService          nativeAdsService;
        private IAdMobNativeAdsService     adMobNativeAdsService;
        private AdServicesConfig           adServicesConfig;
        private ILogger                    logger;
        private int                        loadedCount;
        private bool                       isShow;
        private Action                     onHide;
        private CancellationTokenSource    showingCts;

        private void Awake()
        {
            var container = this.GetCurrentContainer();
            this.signalBus             = container.Resolve<SignalBus>();
            this.adServiceWrapper      = container.Resolve<UITemplateAdServiceWrapper>();
            this.nativeAdsService      = container.Resolve<INativeAdsService>();
            this.adServicesConfig      = container.Resolve<AdServicesConfig>();
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
            this.onHide = signal.OnHide;
            if (signal.IsShow)
            {
                this.btnClose.interactable = false;
                this.OnShow();
                UniTask.WaitForSeconds(this.adServicesConfig.NativeCollapseCloseTime, cancellationToken: (this.showingCts = new()).Token).ContinueWith(this.SetupNativeClick);
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
                this.boxCollider.enabled = true;
                this.logger.Info("Showing native collapse");
                this.adServiceWrapper.HideBannerAd();
                this.view.SetActive(true);
                this.nativeAdsView.Init(this.nativeAdsService);
            }
            else
            {
                this.onHide?.Invoke();
            }
        }

        private void OnHide()
        {
            this.onHide?.Invoke();
            if (!this.isShow) return;
            this.showingCts?.Cancel();
            this.showingCts?.Dispose();
            this.showingCts = null;
            this.logger.Info("Hiding native collapse");
            this.adServiceWrapper.ShowBannerAd();
            this.isShow = false;
            this.view.SetActive(false);
            this.loadedCount++;
            if (this.loadedCount == this.adServicesConfig.NativeCollapseLoad)
            {
                this.loadedCount = 0;
                this.nativeAdsView.Release();
            }
        }

        private void SetupNativeClick()
        {
            this.btnClose.interactable = true;
            this.boxCollider.enabled   = false;
        }
    }
}
#endif