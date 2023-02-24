namespace TheOneStudio.UITemplate.UITemplate.ThirdPartyServices
{
    using System;
    using Core.AdsServices;
    using GameFoundation.Scripts.Utilities.LogService;
    using TheOneStudio.UITemplate.UITemplate.Signals;
    using Zenject;

    public class AdServiceWrapper : IAdServices, IAOAAdService
    {
        #region inject

        private readonly IAdServices   adServices;
        private readonly IAOAAdService aoaAdService;
        private readonly ILogService   logService;
        private readonly SignalBus     signalBus;

        #endregion

        public AdServiceWrapper(IAOAAdService aoaAdService, ILogService logService, SignalBus signalBus, IAdServices adServices)
        {
            this.adServices   = adServices;
            this.aoaAdService = aoaAdService;
            this.logService   = logService;
            this.signalBus    = signalBus;
        }

        public void                                        GrantDataPrivacyConsent()                                                    { this.adServices.GrantDataPrivacyConsent(); }
        public void                                        RevokeDataPrivacyConsent()                                                   { this.adServices.RevokeDataPrivacyConsent(); }
        public void                                        GrantDataPrivacyConsent(AdNetwork adNetwork)                                 { this.adServices.GrantDataPrivacyConsent(adNetwork); }
        public void                                        RevokeDataPrivacyConsent(AdNetwork adNetwork)                                { this.adServices.RevokeDataPrivacyConsent(adNetwork); }
        public ConsentStatus                               GetDataPrivacyConsent(AdNetwork adNetwork)                                   { return this.adServices.GetDataPrivacyConsent(adNetwork); }
        public void                                        ShowBannerAd(BannerAdsPosition bannerAdsPosition = BannerAdsPosition.Bottom) { this.adServices.ShowBannerAd(bannerAdsPosition); }
        public void                                        HideBannedAd()                                                               { this.adServices.HideBannedAd(); }
        public void                                        DestroyBannerAd()                                                            { this.adServices.DestroyBannerAd(); }
        public event Action<InterstitialAdNetwork, string> InterstitialAdCompleted;
        public bool                                        IsInterstitialAdReady() { return this.adServices.IsInterstitialAdReady(); }

        public void ShowInterstitialAd(string place)
        {
            if (!this.IsInterstitialAdReady())
            {
                this.logService.Warning("InterstitialAd was not loaded");

                return;
            }

            this.signalBus.Fire(new InterstitialAdShowedSignal(place));
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

            this.signalBus.Fire(new RewardedAdShowedSignal(place));
            this.adServices.ShowRewardedAd(place);
        }

        public void                                        ShowRewardedAd(string place, Action onCompleted) { this.adServices.ShowRewardedAd(place, onCompleted); }
        public event Action<InterstitialAdNetwork, string> RewardedInterstitialAdCompleted;
        public event Action<InterstitialAdNetwork, string> RewardedInterstitialAdSkipped;
        public bool                                        IsRewardedInterstitialAdReady()                              { return this.adServices.IsInterstitialAdReady(); }
        public void                                        ShowRewardedInterstitialAd(string place)                     { this.adServices.ShowRewardedInterstitialAd(place); }
        public void                                        ShowRewardedInterstitialAd(string place, Action onCompleted) { this.adServices.ShowRewardedInterstitialAd(place, onCompleted); }
        public event Action                                AdsRemoved;
        public void                                        RemoveAds(bool revokeConsent = false) { this.adServices.RemoveAds(revokeConsent); }
        public bool                                        IsAdsInitialized()                    { return this.adServices.IsAdsInitialized(); }

        public bool                                        IsAppOpenAdLoaded() { return this.aoaAdService.IsAppOpenAdLoaded(); }
        public void                                        ShowAppOpenAd()     { this.aoaAdService.ShowAppOpenAd(); }
    }
}