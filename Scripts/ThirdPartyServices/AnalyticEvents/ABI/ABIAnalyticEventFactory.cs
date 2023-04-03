#if ABI
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

    public class ABIAnalyticEventFactory : BaseAnalyticEventFactory
    {
        private readonly SignalBus         signalBus;
        private readonly IAnalyticServices analyticEvents;

        public override IEvent InterstitialEligible(string place) => new AdsIntersEligible(place);

        public override IEvent InterstitialShow(int level, string place) => new AdInterShow(place);

        public override IEvent InterstitialShowCompleted(int level, string place) => new CustomEvent();

        public override IEvent InterstitialShowFail(string place, string msg) => new AdInterFail(msg);

        public override IEvent InterstitialClick(string place) => new AdInterClick(place);

        public override IEvent InterstitialDownloaded(string place) => new AdInterDownloaded();

        public override IEvent InterstitialCalled(string place) => new AdInterCalled();

        public override IEvent RewardedVideoEligible(string place) => new AdsRewardEligible(place);

        public override IEvent RewardedVideoOffer(string place) => new AdsRewardOffer(place);

        public override IEvent RewardedVideoDownloaded(string place) => new AdsRewardedDownloaded();

        public override IEvent RewardedVideoCalled(string place) => new AdsRewardedCalled();

        public override IEvent RewardedVideoShow(int level, string place) => new AdsRewardShow(place);

        public override IEvent RewardedVideoShowCompleted(int level, string place, bool isRewarded)
        {
            if (!isRewarded) return new CustomEvent();

            return new AdsRewardComplete(place);
        }

        public override IEvent RewardedVideoClick(string place) => new AdsRewardClick(place);

        public override IEvent RewardedVideoShowFail(string place, string msg) => new AdsRewardFail(place, msg);

        public override IEvent LevelLose(int level, int timeSpent, int loseCount) => new LevelFail(level, loseCount);

        public override IEvent LevelStart(int level, int gold) => new LevelStart(level, gold);

        public override IEvent LevelWin(int level, int timeSpent, int winCount) => new LevelComplete(level, timeSpent);

        public override IEvent FirstWin(int level, int timeSpent) => new CustomEvent
        {
            EventName = $"checkpoints_{level}",
            EventProperties = new Dictionary<string, object>()
        };

        public override IEvent LevelSkipped(int level, int timeSpent) => new LevelSkipped(level, timeSpent);

        public override IEvent EarnVirtualCurrency(string virtualCurrencyName, long value, string source) => new EarnVirtualCurrency(virtualCurrencyName, value, source);

        public override IEvent SpendVirtualCurrency(string virtualCurrencyName, long value, string itemName) => new SpendVirtualCurrency(virtualCurrencyName, value, itemName);

        public override IEvent TutorialCompletion(bool success, string tutorialId) => new GameTutorialCompletion(success, tutorialId);

        public override void ForceUpdateAllProperties() { }

        public override string LevelMaxProperty             => "level_max";
        public override string LastLevelProperty            => "last_level";
        public override string LastAdsPlacementProperty     => "last_placement";
        public override string TotalInterstitialAdsProperty => "total_interstitial_ads";
        public override string TotalRewardedAdsProperty     => "total_rewarded_ads";

        public override AnalyticsEventCustomizationConfig AppsFlyerAnalyticsEventCustomizationConfig { get; set; } = new()
        {
            IgnoreEvents = new HashSet<Type>()
            {
                typeof(GameStarted),
                typeof(AdInterClick),
                typeof(AdInterFail),
                typeof(AdInterDownloaded),
                typeof(AdsRewardFail),
                typeof(AdsRewardOffer),
                typeof(AdsRewardedDownloaded),
                typeof(AdsRewardClick),
            },
            CustomEventKeys = new Dictionary<string, string>()
            {
                { nameof(BannerShown), "af_banner_shown" },
                { nameof(GameTutorialCompletion), "af_tutorial_completion" },
                { nameof(LevelComplete), "af_level_achieved" },
                { nameof(AdsIntersEligible), "af_inters_ad_eligible" },
                { nameof(AdInterCalled), "af_inters_api_called" },
                { nameof(AdInterShow), "af_inters_displayed" },
                { nameof(AdsRewardEligible), "af_rewarded_ad_eligible" },
                { nameof(AdsRewardedCalled), "af_rewarded_api_called" },
                { nameof(AdsRewardShow), "af_rewarded_displayed" },
                { nameof(AdsRewardComplete), "af_rewarded_ad_completed" },
            }
        };

        public override AnalyticsEventCustomizationConfig FireBaseAnalyticsEventCustomizationConfig { get; set; } = new()
        {
            IgnoreEvents = new HashSet<Type>(),
            CustomEventKeys = new Dictionary<string, string>()
            {
                { nameof(AdsRevenueEvent), "ad_impression_abi" }
            }
        };

        public  ABIAnalyticEventFactory(SignalBus signalBus, IAnalyticServices analyticEvents)
        {
            this.signalBus = signalBus;
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
#endif