namespace TheOneStudio.UITemplate.UITemplate.Scripts.ThirdPartyServices.AnalyticEvents.ABI
{
    using System;
    using System.Collections.Generic;
    using Core.AnalyticServices;
    using Core.AnalyticServices.CommonEvents;
    using Core.AnalyticServices.Data;
    using Core.AnalyticServices.Signal;
    using TheOneStudio.UITemplate.UITemplate.ThirdPartyServices.AnalyticEvents;
    using TheOneStudio.UITemplate.UITemplate.ThirdPartyServices.AnalyticEvents.ABI;
    using Zenject;

    public class ABIAnalyticEventFactory : IAnalyticEventFactory
    {
        private readonly SignalBus         signalBus;
        private readonly IAnalyticServices analyticEvents;
        public           IEvent            InterstitialShow(int level, string place) => new AdInterLoad(place);

        public IEvent InterstitialShowCompleted(int level, string place) => new AdInterShow(place);

        public IEvent InterstitialShowFail(string place, string msg) => new AdInterFail(msg);

        public IEvent InterstitialClick(string place) => new AdInterClick(place);

        public IEvent InterstitialRequest(string place) => new AdInterRequest();

        public IEvent RewardedVideoOffer(string place) => new AdsRewardOffer(place);

        public IEvent RewardedVideoLoaded(string place) => new AdsRewardedLoaded();

        public IEvent RewardedVideoShow(int level, string place) => new AdsRewardOffer(place);

        public IEvent RewardedVideoShowCompleted(int level, string place, bool isRewarded) => new AdsRewardComplete(place);

        public IEvent RewardedVideoClick(string place) => new AdsRewardClick(place);

        public IEvent RewardedVideoShowFail(string place, string msg) => new AdsRewardFail(place, msg);

        public IEvent LevelLose(int level, int timeSpent, int loseCount) => new LevelFail(level, loseCount);

        public IEvent LevelStart(int level, int gold) => new LevelStart(level, gold);

        public IEvent LevelWin(int level, int timeSpent, int winCount) => new LevelComplete(level, timeSpent);

        public IEvent FirstWin(int level, int timeSpent) => new CustomEvent
        {
            EventName       = $"checkpoints_{level}",
            EventProperties = new Dictionary<string, object>()
        };

        public IEvent LevelSkipped(int level, int timeSpent) => new LevelSkipped(level, timeSpent);

        public IEvent EarnVirtualCurrency(string virtualCurrencyName, long value, string source) => new EarnVirtualCurrency(virtualCurrencyName, value, source);

        public IEvent SpendVirtualCurrency(string virtualCurrencyName, long value, string itemName) => new SpendVirtualCurrency(virtualCurrencyName, value, itemName);

        public void ForceUpdateAllProperties() { }

        public string LevelMaxProperty             => "level_max";
        public string LastLevelProperty            => "last_level";
        public string LastAdsPlacementProperty     => "last_placement";
        public string TotalInterstitialAdsProperty => "total_interstitial_ads";
        public string TotalRewardedAdsProperty     => "total_rewarded_ads";

        public AnalyticsEventCustomizationConfig AppsFlyerAnalyticsEventCustomizationConfig { get; set; } = new()
        {
            IgnoreEvents = new HashSet<Type>()
            {
                typeof(GameStarted),
                typeof(AdInterClick),
                typeof(AdInterFail),
                typeof(AdsRewardFail),
                typeof(AdsRewardOffer),
            },
            CustomEventKeys = new Dictionary<string, string>()
            {
                { nameof(BannerShown), "af_banner_shown" },
                { nameof(LevelComplete), "af_level_achieved" },
                { nameof(AdInterLoad), "af_inters_api_called" },
                { nameof(AdInterShow), "af_inters_displayed" },
                { nameof(AdInterRequest), "af_inters_ad_eligible" },
                { nameof(AdsRewardClick), "af_rewarded_ad_eligible" },
                { nameof(AdsRewardedLoaded), "af_rewarded_api_called" },
                { nameof(AdsRewardShow), "af_rewarded_displayed" },
                { nameof(AdsRewardComplete), "af_rewarded_ad_completed" },
            }
        };

        public AnalyticsEventCustomizationConfig FireBaseAnalyticsEventCustomizationConfig { get; set; } = new()
        {
            IgnoreEvents = new HashSet<Type>(),
            CustomEventKeys = new Dictionary<string, string>()
            {
                { nameof(AdsRevenueEvent), "ad_impression_abi" }
            }
        };

        public ABIAnalyticEventFactory(SignalBus signalBus, IAnalyticServices analyticEvents)
        {
            this.signalBus      = signalBus;
            this.analyticEvents = analyticEvents;
            this.signalBus.Subscribe<AdRevenueSignal>(this.OnAdRevenueEvent);
        }

        private void OnAdRevenueEvent(AdRevenueSignal obj)
        {
            this.analyticEvents.Track(new CustomEvent()
            {
                EventName = "ad_impression",
                EventProperties = new Dictionary<string, object>()
                {
                    { "source_id", obj.AdsRevenueEvent.AdsRevenueSourceId },
                    { "ad_network", obj.AdsRevenueEvent.AdNetwork },
                    { "ad_unit", obj.AdsRevenueEvent.AdUnit },
                    { "placement", obj.AdsRevenueEvent.Placement },
                    { "currency", obj.AdsRevenueEvent.Currency },
                    { "revenue", obj.AdsRevenueEvent.Revenue },
                }
            });
        }
    }
}