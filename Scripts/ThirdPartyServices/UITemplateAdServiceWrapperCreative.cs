namespace TheOneStudio.UITemplate.UITemplate.Scripts.ThirdPartyServices
{
    using System;
    using System.Collections.Generic;
    using Core.AdsServices;
    using GameFoundation.Scripts.Utilities.LogService;
    using TheOneStudio.UITemplate.UITemplate.Models;
    using Zenject;

    public class UITemplateAdServiceWrapperCreative : UITemplateAdServiceWrapper
    {
        public UITemplateAdServiceWrapperCreative(ILogService logService, SignalBus signalBus, IAdServices adServices, List<IMRECAdService> mrecAdServices, UITemplateAdsData uiTemplateAdsData,
                                                  UITemplateAdServiceConfig config, IAOAAdService aoaAdService) : base(logService, signalBus, adServices, mrecAdServices, uiTemplateAdsData, config,
                                                                                                                       aoaAdService)
        {
        }

        public override void ShowBannerAd() { }
        public override void HideBannerAd() { }

        public override bool ShowInterstitialAd(string place, bool force = false) { return true; }

        public override void ShowRewardedAd(string place, Action onComplete) { onComplete.Invoke(); }

        public override void RewardedAdOffer(string place) { }

        public override bool IsRewardedAdReady(string place) { return true; }

        public override bool IsInterstitialAdReady(string place) { return true; }

        public override void ShowMREC(AdViewPosition adViewPosition) { }

        public override void HideMREC(AdViewPosition adViewPosition) { }
    }
}