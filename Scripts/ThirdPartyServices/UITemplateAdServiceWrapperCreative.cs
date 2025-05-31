namespace TheOneStudio.UITemplate.UITemplate.Scripts.ThirdPartyServices
{
    #if CREATIVE
    using System;
    using System.Collections.Generic;
    using Core.AdsServices;
    using Core.AdsServices.CollapsibleBanner;
    using Core.AdsServices.Native;
    using Cysharp.Threading.Tasks;
    using GameFoundation.Scripts.UIModule.ScreenFlow.Managers;
    using GameFoundation.Scripts.Utilities.LogService;
    using GameFoundation.Signals;
    using ServiceImplementation.AdsServices.ConsentInformation;
    using ServiceImplementation.AdsServices.NativeOverlay;
    using ServiceImplementation.AdsServices.PreloadService;
    using ServiceImplementation.Configs;
    using ServiceImplementation.Configs.Ads;
    using TheOneStudio.UITemplate.UITemplate.Models.Controllers;
    using TheOneStudio.UITemplate.UITemplate.Services.Toast;
    using UnityEngine.Scripting;

    public class UITemplateAdServiceWrapperCreative : UITemplateAdServiceWrapper
    {
        [Preserve]
        public UITemplateAdServiceWrapperCreative(
            ILogService                         logService,
            AdServicesConfig                    adServicesConfig,
            SignalBus                           signalBus,
            IEnumerable<IAdServices>            adServices,
            IEnumerable<IMRECAdService>         mrecAdServices,
            UITemplateAdsController             uiTemplateAdsController,
            UITemplateGameSessionDataController gameSessionDataController,
            IEnumerable<IAOAAdService>          aoaAdServices,
            ToastController                     toastController,
            UITemplateLevelDataController       levelDataController,
            ThirdPartiesConfig                  thirdPartiesConfig,
            IScreenManager                      screenManager,
            ICollapsibleBannerAd                collapsibleBannerAd,
            IEnumerable<AdServiceOrder>         adServiceOrders,
            IConsentInformation                 consentInformation,
            IEnumerable<INativeAdsService>      nativeAdsServices,
            PreloadAdService                    preloadAdService,
            INativeOverlayService               nativeOverlayService
        ) : base(logService,
            adServicesConfig,
            signalBus,
            adServices,
            mrecAdServices,
            uiTemplateAdsController,
            gameSessionDataController,
            aoaAdServices,
            toastController,
            levelDataController,
            thirdPartiesConfig,
            screenManager,
            collapsibleBannerAd,
            adServiceOrders,
            consentInformation,
            nativeAdsServices,
            preloadAdService,
            nativeOverlayService)
        {
        }

        public override void ShowBannerAd(int width = 320, int height = 50, bool forceShowMediation = false)
        {
        }

        public override void HideBannerAd()
        {
        }

        public override bool ShowInterstitialAd(string place, Action<bool> onInterstitialFinished, bool force = false)
        {
            onInterstitialFinished?.Invoke(true);

            return false;
        }

        public override void ShowRewardedAd(string place, Action onComplete, Action onFail = null)
        {
            onComplete.Invoke();
        }

        public override void RewardedAdOffer(string place)
        {
        }

        public override bool IsRewardedAdReady(string place)
        {
            return true;
        }

        public override bool IsInterstitialAdReady(string place)
        {
            return true;
        }

        public override void HideMREC(string placement, AdScreenPosition position)
        {
        }

        public override void ShowMREC(string placement, AdScreenPosition position, AdScreenPosition offset = default)
        {
        }

        protected async override UniTaskVoid ShowAOAAdsIfAvailable(bool isOpenAppAOA)
        {
        }

        #if THEONE_COLLAPSIBLE_MREC
        public override void ShowCollapsibleMREC(string placement)
        {
        }

        public override void HideCollapsibleMREC(string placement)
        {

        }
        #endif

        #if ADMOB

        public override void ShowNativeOverlayAd(string placement, AdViewPosition adViewPosition)
        {
        }

        public override void HideNativeOverlayAd(string placement)
        {
        }

        public override void ShowNativeOverlayInterAd(string placement, Action<bool> onComplete, bool isHidePreviousMrec = true)
        {
            this.ShowInterstitialAd(placement, onComplete);
        }

        #endif
    }
    #endif
}