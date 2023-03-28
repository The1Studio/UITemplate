namespace TheOneStudio.UITemplate.UITemplate.Scripts.ThirdPartyServices
{
    using System;
    using Core.AdsServices;
    using Core.AdsServices.Signals;
    using Cysharp.Threading.Tasks;
    using GameFoundation.Scripts.Utilities.LogService;
    using TheOneStudio.UITemplate.UITemplate.Models;
    using TheOneStudio.UITemplate.UITemplate.Scripts.Signals;
    using TheOneStudio.UITemplate.UITemplate.Signals;
    using Zenject;

    public class UITemplateAdServiceConfig
    {
        public long InterstitialAdInterval { get; set; }
    }

    public class UITemplateAdServiceWrapper
    {
        #region inject

        private readonly IAdServices               adServices;
        private readonly IMRECAdService            mrecAdService;
        private readonly UITemplateAdsData         uiTemplateAdsData;
        private readonly UITemplateAdServiceConfig config;
        private readonly ILogService               logService;
        private readonly SignalBus                 signalBus;

        #endregion

        private long lastInterstitialAdTime;

        public UITemplateAdServiceWrapper(ILogService logService, SignalBus signalBus, IAdServices adServices, IMRECAdService mrecAdService, UITemplateAdsData uiTemplateAdsData,
            UITemplateAdServiceConfig config)
        {
            this.adServices        = adServices;
            this.mrecAdService     = mrecAdService;
            this.uiTemplateAdsData = uiTemplateAdsData;
            this.config            = config;
            this.logService        = logService;
            this.signalBus         = signalBus;
        }

        public virtual async void ShowBannerAd()
        {
            await UniTask.WaitUntil(() => this.adServices.IsAdsInitialized());
            this.adServices.ShowBannerAd();
        }

        #region InterstitialAd

        public virtual bool IsInterstitialAdReady(string place) { return this.adServices.IsInterstitialAdReady(place); }

        public virtual void ShowInterstitialAd(string place, bool force = false)
        {
            var currentTimestamp = new DateTimeOffset(DateTime.Now).ToUnixTimeSeconds();
            if (this.lastInterstitialAdTime + this.config.InterstitialAdInterval > currentTimestamp && !force)
            {
                this.logService.Warning("InterstitialAd was not passed interval");

                return;
            }

            this.lastInterstitialAdTime = currentTimestamp;

            this.signalBus.Fire(new InterstitialAdEligibleSignal(place));
            if (!this.adServices.IsInterstitialAdReady(place))
            {
                this.logService.Warning("InterstitialAd was not loaded");

                return;
            }

            this.signalBus.Fire(new InterstitialAdCalledSignal(place));
            this.uiTemplateAdsData.WatchedInterstitialAds++;
            this.adServices.ShowInterstitialAd(place);
        }

        public virtual void LoadInterstitialAd(string place) { this.signalBus.Fire(new InterstitialAdDownloadedSignal(place)); }

        #endregion

        #region RewardAd

        public virtual void ShowRewardedAd(string place, Action onComplete)
        {
            this.signalBus.Fire(new RewardedAdEligibleSignal(place));
            if (!this.adServices.IsRewardedAdReady(place))
            {
                this.logService.Warning("Rewarded was not loaded");

                return;
            }

            this.signalBus.Fire(new RewardedAdCalledSignal(place));
            this.uiTemplateAdsData.WatchedRewardedAds++;
            this.adServices.ShowRewardedAd(place, onComplete);
        }

        public virtual bool IsRewardedAdReady(string place) { return this.adServices.IsRewardedAdReady(place); }

        public virtual void RewardedAdOffer(string place) { this.signalBus.Fire(new RewardedAdOfferSignal(place)); }

        #endregion

        public virtual void ShowMREC(AdViewPosition adViewPosition)
        {
            if (adViewPosition == AdViewPosition.BottomCenter)
            {
                this.adServices.HideBannedAd();
            }

            this.mrecAdService.ShowMREC(adViewPosition);
        }

        public virtual void HideMREC(AdViewPosition adViewPosition)
        {
            this.mrecAdService.HideMREC(adViewPosition);

            if (adViewPosition == AdViewPosition.BottomCenter)
            {
                this.adServices.ShowBannerAd();
            }
        }
    }
}