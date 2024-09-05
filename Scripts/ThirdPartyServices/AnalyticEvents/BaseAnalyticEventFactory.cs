namespace TheOneStudio.UITemplate.UITemplate.ThirdPartyServices.AnalyticEvents
{
    using System.Collections.Generic;
    using Core.AnalyticServices;
    using Core.AnalyticServices.CommonEvents;
    using Core.AnalyticServices.Data;
    using Core.AnalyticServices.Signal;
    using GameFoundation.Signals;
    using TheOneStudio.UITemplate.UITemplate.ThirdPartyServices.AnalyticEvents.CommonEvents;

    public abstract class BaseAnalyticEventFactory : IAnalyticEventFactory
    {
        //Interstitial ads
        public virtual IEvent BannerAdShow() => new BannerShown();

        public virtual IEvent BannerAdLoadFail(string msg) => new BannerLoadFail(msg);

        public virtual IEvent BannerAdLoad() => new BannerLoad();

        public virtual IEvent InterstitialEligible(string place) => new InterstitialAdEligible(place);

        public virtual IEvent InterstitialShow(int level, string place) => new InterstitialAdDisplayed(level, place);

        public virtual IEvent InterstitialShowCompleted(int level, string place) => new InterstitialAdClosed(place);

        public virtual IEvent InterstitialShowFail(string place, string msg) => new InterstitialAdDisplayedFailed(place);

        public virtual IEvent InterstitialClick(string place) => new InterstitialAdClicked(place);

        public virtual IEvent InterstitialDownloaded(string place, long loadingMilis)     => new InterstitialAdDownloaded(place, loadingMilis);

        public virtual IEvent InterstitialDownloadFailed(string place, string message, long loadingMilis) => new InterstitialAdLoadFailed(place, message, loadingMilis);

        public virtual IEvent InterstitialCalled(string place) => new InterstitialAdCalled(place);

        //Rewarded Interstitital
        public virtual IEvent RewardedInterstitialAdDisplayed(int level, string place) => new CustomEvent();

        //RewardVideo Ads
        public virtual IEvent RewardedVideoEligible(string place) => new RewardedAdEligible(place);

        public virtual IEvent RewardedVideoOffer(string place)                         => new RewardedAdOffer(place);

        public virtual IEvent RewardedVideoDownloaded(string place, long loadingMilis) => new RewardedAdLoaded(place, loadingMilis);

        public virtual IEvent RewardedVideoDownloadFailed(string place, long loadingMilis) => new RewardedAdLoadFailed(place, loadingMilis, "");

        public virtual IEvent RewardedVideoCalled(string place) => new RewardedAdCalled(place);

        public virtual IEvent RewardedVideoShow(int level, string place) => new RewardedAdDisplayed(place, level);

        public IEvent RewardedLoadFail(string place, string msg) => new RewardedAdLoadFailed(place,0, msg);

        public virtual IEvent RewardedVideoShowCompleted(int level, string place, bool isRewarded)
        {
            return isRewarded ? new RewardedAdCompleted(place) : new RewardedSkipped(place);
        }

        public virtual IEvent RewardedVideoClick(string place) { return new RewardedAdLoadClicked(place); }

        public virtual IEvent RewardedVideoShowFail(string place, string msg) => new RewardedAdShowFail(place);

        //App open
        public virtual IEvent AppOpenCalled()                  => new AppOpenCalled();
        public virtual IEvent AppOpenEligible()                => new AppOpenEligible();
        public virtual IEvent AppOpenLoadFailed()              => new AppOpenLoadFailed();
        public virtual IEvent AppOpenLoaded()                  => new AppOpenLoaded();
        public virtual IEvent AppOpenFullScreenContentClosed() => new AppOpenFullScreenContentClosed();
        public virtual IEvent AppOpenFullScreenContentFailed() => new AppOpenFullScreenContentFailed();
        public virtual IEvent AppOpenFullScreenContentOpened() => new AppOpenFullScreenContentOpened();
        public virtual IEvent AppOpenClicked()                 => new AppOpenClicked();

        //Level
        public virtual IEvent LevelStart(int level, int gold) => new LevelStart(level, gold);

        public virtual IEvent LevelWin(int level, int timeSpent, int winCount) => new LevelWin(level, timeSpent);

        public virtual IEvent LevelLose(int level, int timeSpent, int loseCount) => new LevelLose(level, timeSpent);

        public virtual IEvent FirstWin(int level, int timeSpent) => new FirstWin(level, timeSpent);

        public virtual IEvent LevelSkipped(int level, int timeSpent) => new LevelSkipped(level, timeSpent);

        public virtual IEvent EarnVirtualCurrency(string virtualCurrencyName, long value, string source) => new EarnVirtualCurrency(virtualCurrencyName, value, source);

        public virtual IEvent SpendVirtualCurrency(string virtualCurrencyName, long value, string itemName) => SpendVirtualCurrency(virtualCurrencyName, value, itemName);

        public virtual IEvent TutorialCompletion(bool success, string tutorialId) => new TutorialCompletion(success, tutorialId);

        public virtual void ForceUpdateAllProperties() { }

        public virtual string LevelMaxProperty                   => "level_max";
        public virtual string LastLevelProperty                  => "last_level";
        public virtual string LastAdsPlacementProperty           => "last_placement";
        public virtual string TotalInterstitialAdsProperty       => "total_interstitial_ads";
        public virtual string TotalRewardedAdsProperty           => "total_rewarded_ads";
        public virtual string TotalVirtualCurrencySpentProperty  => "total_spent";
        public virtual string TotalVirtualCurrencyEarnedProperty => "total_earned";
        public virtual string DaysPlayedProperty                 => "days_played";
        public virtual string RetentionDayProperty               => "retention";

        public virtual AnalyticsEventCustomizationConfig AppsFlyerAnalyticsEventCustomizationConfig { get; set; } = new();
        public virtual AnalyticsEventCustomizationConfig FireBaseAnalyticsEventCustomizationConfig { get; set; } = new();
        public virtual AnalyticsEventCustomizationConfig ByteBrewAnalyticsEventCustomizationConfig { get; set; } = new();

        protected virtual string EvenName { get; } = "ad_impression";

        protected readonly IAnalyticServices analyticServices;

        protected BaseAnalyticEventFactory(SignalBus signalBus, IAnalyticServices analyticServices)
        {
            this.analyticServices = analyticServices;
            signalBus.Subscribe<AdRevenueSignal>(this.OnAdsRevenue);
        }

        private void OnAdsRevenue(AdRevenueSignal obj)
        {
            this.analyticServices.Track(new CustomEvent()
            {
                EventName = this.EvenName,
                EventProperties = new()
                {
                    { "ad_platform", obj.AdsRevenueEvent.AdsRevenueSourceId },
                    { "ad_source", obj.AdsRevenueEvent.AdNetwork },
                    { "ad_unit_name", obj.AdsRevenueEvent.AdUnit },
                    { "ad_format", obj.AdsRevenueEvent.AdFormat },
                    { "placement", obj.AdsRevenueEvent.Placement },
                    { "currency", obj.AdsRevenueEvent.Currency },
                    { "value", obj.AdsRevenueEvent.Revenue },
                }
            });
        }
    }
}