namespace TheOneStudio.UITemplate.UITemplate.ThirdPartyServices.AnalyticEvents
{
    using System.Collections.Generic;
    using Core.AnalyticServices;
    using Core.AnalyticServices.CommonEvents;
    using Core.AnalyticServices.Data;
    using Core.AnalyticServices.Signal;
    using TheOneStudio.UITemplate.UITemplate.ThirdPartyServices.AnalyticEvents.ABI;
    using TheOneStudio.UITemplate.UITemplate.ThirdPartyServices.AnalyticEvents.Rocket;
    using Zenject;
    using LevelSkipped = TheOneStudio.UITemplate.UITemplate.ThirdPartyServices.AnalyticEvents.Rocket.LevelSkipped;
    using LevelStart = TheOneStudio.UITemplate.UITemplate.ThirdPartyServices.AnalyticEvents.Rocket.LevelStart;

    public abstract class BaseAnalyticEventFactory : IAnalyticEventFactory
    {
        public virtual IEvent InterstitialEligible(string place) => new CustomEvent();

        public virtual IEvent InterstitialShow(int level, string place) => new InterstitialShow(level, place);

        public virtual IEvent InterstitialShowCompleted(int level, string place) => new InterstitialShowCompleted(place);

        public virtual IEvent InterstitialShowFail(string place, string msg) => new CustomEvent();

        public virtual IEvent InterstitialClick(string place) => new CustomEvent();

        public virtual IEvent InterstitialDownloaded(string place) => new CustomEvent();

        public virtual IEvent InterstitialCalled(string place) => new CustomEvent();

        public IEvent RewardedInterstitialAdDisplayed(int level, string place) => new CustomEvent();

        public virtual IEvent RewardedVideoEligible(string place) => new CustomEvent();

        public virtual IEvent RewardedVideoOffer(string place) => new CustomEvent();

        public virtual IEvent RewardedVideoDownloaded(string place) => new CustomEvent();

        public virtual IEvent RewardedVideoCalled(string place) => new CustomEvent();

        public virtual IEvent RewardedVideoShow(int level, string place) => new RewardedVideoShow(level, place);

        public virtual IEvent RewardedVideoShowCompleted(int level, string place, bool isRewarded) => new RewardedAdsShowCompleted(place, isRewarded ? "success" : "skip");

        public virtual IEvent RewardedVideoClick(string place) { return new CustomEvent(); }

        public virtual IEvent RewardedVideoShowFail(string place, string msg) { return new CustomEvent(); }

        public virtual IEvent LevelLose(int level, int timeSpent, int loseCount) => new LevelLose(level, timeSpent);

        public virtual IEvent LevelStart(int level, int gold) => new LevelStart(level);

        public virtual IEvent LevelWin(int level, int timeSpent, int winCount) => new LevelWin(level, timeSpent);

        public virtual IEvent FirstWin(int level, int timeSpent) => new CustomEvent();

        public virtual IEvent LevelSkipped(int level, int timeSpent) => new LevelSkipped(level, timeSpent);

        public virtual IEvent EarnVirtualCurrency(string virtualCurrencyName, long value, string source) { return new CustomEvent(); }

        public virtual IEvent SpendVirtualCurrency(string virtualCurrencyName, long value, string itemName) { return new CustomEvent(); }

        public virtual IEvent TutorialCompletion(bool success, string tutorialId) { return new CustomEvent(); }

        public virtual IEvent EarnVirtualCurrency(string type, int amount) { return new CustomEvent(); }

        public virtual IEvent BuildingUnlock(bool success) => new BuildingUnlock(success);

        public virtual void ForceUpdateAllProperties() { }

        public virtual string LevelMaxProperty                   => "level_max";
        public virtual string LastLevelProperty                  => "last_level";
        public virtual string LastAdsPlacementProperty           => "last_placement";
        public virtual string TotalInterstitialAdsProperty       => "total_interstitial_ads";
        public virtual string TotalRewardedAdsProperty           => "total_rewarded_ads";
        public virtual string TotalVirtualCurrencySpentProperty  => "total_spent";
        public virtual string TotalVirtualCurrencyEarnedProperty => "total_earned";
        public virtual string DaysPlayedProperty                 => "days_played";

        public virtual AnalyticsEventCustomizationConfig AppsFlyerAnalyticsEventCustomizationConfig { get; set; } = new();

        public virtual AnalyticsEventCustomizationConfig FireBaseAnalyticsEventCustomizationConfig { get; set; } = new();

        protected virtual string EvenName { get; } = "ad_impression";

        private readonly IAnalyticServices analyticServices;

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
                EventProperties = new Dictionary<string, object>()
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