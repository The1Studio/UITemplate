namespace TheOneStudio.UITemplate.UITemplate.Scripts.ThirdPartyServices
{
    using System;
    using Core.AdsServices;
    using GameFoundation.Scripts.Utilities.LogService;
    using TheOneStudio.UITemplate.UITemplate.Models;
    using Zenject;

    public class UITemplateAdServiceWrapperCreative : UITemplateAdServiceWrapper
    {
        public UITemplateAdServiceWrapperCreative(ILogService logService, SignalBus signalBus, IAdServices adServices, IMRECAdService mrecAdService, UITemplateAdsData uiTemplateAdsData) : base(logService, signalBus, adServices, mrecAdService, uiTemplateAdsData)
        {
        }
        
        public override void ShowBannerAd()                                         { }
        public override void ShowInterstitialAd(string    place)                    {  }
        public override void ShowRewardedAd(string place, Action onComplete)
        {
            onComplete.Invoke();
        }
        public override bool IsRewardedAdReady(string     place) { return true; }
        public override bool IsInterstitialAdReady(string place) { return true; }

        public override void ShowMREC(AdViewPosition adViewPosition)
        {
        }
        public override void HideMREC(AdViewPosition adViewPosition)
        {
        }
    }
}