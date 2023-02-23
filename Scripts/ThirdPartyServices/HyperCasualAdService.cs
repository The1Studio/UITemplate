namespace TheOneStudio.UITemplate.UITemplate.ThirdPartyServices
{
    using System;
    using System.Collections.Generic;
    using Core.AdsServices;
    using Core.AnalyticServices;
    using Core.AnalyticServices.Data;
    using GameFoundation.Scripts.Utilities.LogService;
    using TheOneStudio.UITemplate.UITemplate.Models;
    using TheOneStudio.UITemplate.UITemplate.ThirdPartyServices.AnalyticEvents.OneSoft;

    public class TheOneAdServiceWrapper : IAdServices, IAOAAdService
    {
        #region inject

        private readonly List<IAnalyticServices> analyticServices;
        private readonly IAdServices             adServices;
        private readonly IAOAAdService           aoaAdService;
        private readonly ILogService             logService;
        private readonly UITemplateUserLevelData uiTemplateUserLevelData;

        #endregion

        public TheOneAdServiceWrapper(List<IAnalyticServices> analyticServices, IAdServices adServices, IAOAAdService aoaAdService, ILogService logService, UITemplateUserLevelData uiTemplateUserLevelData)
        {
            this.analyticServices        = analyticServices;
            this.adServices              = adServices;
            this.aoaAdService            = aoaAdService;
            this.logService              = logService;
            this.uiTemplateUserLevelData = uiTemplateUserLevelData;

            this.analyticServices.ForEach(analyticService => analyticService.Start());
        }

        private void AnalyticTrack(IEvent analyticEvent)
        {
            this.analyticServices.ForEach(analyticService => analyticService.Track(analyticEvent));
        }

        public void                                        GrantDataPrivacyConsent()                                                        { this.adServices.GrantDataPrivacyConsent(); }
        public void                                        RevokeDataPrivacyConsent()                                                       { this.adServices.RevokeDataPrivacyConsent(); }
        public void                                        GrantDataPrivacyConsent(AdNetwork  adNetwork)                                    { this.adServices.GrantDataPrivacyConsent(adNetwork); }
        public void                                        RevokeDataPrivacyConsent(AdNetwork adNetwork)                                    { this.adServices.RevokeDataPrivacyConsent(adNetwork); }
        public ConsentStatus                               GetDataPrivacyConsent(AdNetwork    adNetwork)                                    { return this.adServices.GetDataPrivacyConsent(adNetwork); }
        public void                                        ShowBannerAd(BannerAdsPosition     bannerAdsPosition = BannerAdsPosition.Bottom) { this.adServices.ShowBannerAd(bannerAdsPosition); }
        public void                                        HideBannedAd()                                                                   { this.adServices.HideBannedAd(); }
        public void                                        DestroyBannerAd()                                                                { this.adServices.DestroyBannerAd(); }
        public event Action<InterstitialAdNetwork, string> InterstitialAdCompleted;
        public bool                                        IsInterstitialAdReady() { return this.adServices.IsInterstitialAdReady(); }
        public void ShowInterstitialAd(string place)
        {
            if (!this.IsInterstitialAdReady())
            {
                this.logService.Warning("InterstitialAd was not loaded");
                return;
            }
             
            this.AnalyticTrack(new InterstitialShow(this.uiTemplateUserLevelData.CurrentLevel, place));
            this.adServices.ShowInterstitialAd(place);
        }
        public event Action<InterstitialAdNetwork, string> RewardedAdCompleted;
        public event Action<InterstitialAdNetwork, string> RewardedAdSkipped;
        public bool                                        IsRewardedAdReady() { return this.adServices.IsRewardedAdReady(); }
        public void ShowRewardedAd(string place)
        {
            if (!this.IsRewardedAdReady())
            {
                this.logService.Warning("Rewarded was not loaded");
                return;
            }
            
            this.AnalyticTrack(new RewardedVideoShown(this.uiTemplateUserLevelData.CurrentLevel, place));
            this.adServices.ShowRewardedAd(place);
        }
        public void                                        ShowRewardedAd(string place, Action onCompleted) { this.adServices.ShowRewardedAd(place, onCompleted); }
        public event Action<InterstitialAdNetwork, string> RewardedInterstitialAdCompleted;
        public event Action<InterstitialAdNetwork, string> RewardedInterstitialAdSkipped;
        public bool                                        IsRewardedInterstitialAdReady()          { return this.adServices.IsInterstitialAdReady(); }
        public void                                        ShowRewardedInterstitialAd(string place) { this.adServices.ShowRewardedInterstitialAd(place); }
        public event Action                                AdsRemoved;
        public void                                        RemoveAds(bool revokeConsent = false) { this.adServices.RemoveAds(revokeConsent); }
        public bool                                        IsAppOpenAdLoaded()                   { return this.aoaAdService.IsAppOpenAdLoaded(); }
        public void                                        ShowAppOpenAd()                       { this.aoaAdService.ShowAppOpenAd(); }
    }
}