namespace TheOneStudio.UITemplate.UITemplate.Scripts.ThirdPartyServices
{
    using System;
    using System.Collections.Generic;
    using Core.AdsServices;
    using Core.AdsServices.CollapsibleBanner;
    using GameFoundation.Scripts.UIModule.ScreenFlow.BaseScreen.Presenter;
    using GameFoundation.Scripts.UIModule.ScreenFlow.Managers;
    using GameFoundation.Scripts.Utilities.LogService;
    using ServiceImplementation.Configs;
    using ServiceImplementation.Configs.Ads;
    using TheOneStudio.UITemplate.UITemplate.Models.Controllers;
    using TheOneStudio.UITemplate.UITemplate.Services.Toast;
    using Zenject;

    public class UITemplateAdServiceWrapperCreative : UITemplateAdServiceWrapper
    {
        public UITemplateAdServiceWrapperCreative(ILogService logService, AdServicesConfig adServicesConfig, SignalBus signalBus, IAdServices adServices, List<IMRECAdService> mrecAdServices,
            UITemplateAdsController uiTemplateAdsController, UITemplateGameSessionDataController gameSessionDataController, List<IAOAAdService> aoaAdServices, IBackFillAdsService backFillAdsService,
            ToastController toastController, UITemplateLevelDataController levelDataController, ThirdPartiesConfig thirdPartiesConfig, IScreenManager screenManager, ICollapsibleBannerAd collapsibleBannerAd)
            : base(logService, adServicesConfig, signalBus, adServices, mrecAdServices, uiTemplateAdsController,
                gameSessionDataController, aoaAdServices, backFillAdsService, toastController, levelDataController, thirdPartiesConfig, screenManager, collapsibleBannerAd)
        {
        }

        public override void ShowBannerAd(int width = 320, int height = 50) { }

        public override void HideBannerAd() { }

        public override bool ShowInterstitialAd(string place, Action<bool> onInterstitialFinished, bool force = false)
        {
            onInterstitialFinished?.Invoke(true);

            return false;
        }

        public override void ShowRewardedAd(string place, Action onComplete, Action onFail = null) { onComplete.Invoke(); }

        public override void RewardedAdOffer(string place) { }

        public override bool IsRewardedAdReady(string place) { return true; }

        public override bool IsInterstitialAdReady(string place) { return true; }

        public override void ShowMREC<TPresenter>(AdViewPosition adViewPosition) { }

        public override void HideMREC(AdViewPosition adViewPosition) { }
    }
}