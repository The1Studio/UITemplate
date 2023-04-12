#if ROCKET
namespace TheOneStudio.UITemplate.UITemplate.ThirdPartyServices.AnalyticEvents.Rocket
{
    using System;
    using System.Collections.Generic;
    using Core.AnalyticServices.CommonEvents;
    using Core.AnalyticServices.Data;
    using TheOneStudio.UITemplate.UITemplate.Scripts.ThirdPartyServices.AnalyticEvents.ABI;
    using TheOneStudio.UITemplate.UITemplate.ThirdPartyServices.AnalyticEvents.ABI;

    public class RocketAnalyticEventFactory : BaseAnalyticEventFactory
    {
        public override IEvent InterstitialEligible(string place) => new CustomEvent();

        public override IEvent InterstitialShow(int level, string place) => new InterstitialShow(level, place);

        public override IEvent InterstitialShowCompleted(int level, string place) => new InterstitialShowCompleted(place);

        public override IEvent InterstitialShowFail(string place, string msg) => new CustomEvent();

        public override IEvent InterstitialClick(string place) => new CustomEvent();

        public override IEvent InterstitialDownloaded(string place) => new CustomEvent();

        public override IEvent InterstitialCalled(string place) => new CustomEvent();

        public override IEvent RewardedVideoEligible(string place) => new CustomEvent();

        public override IEvent RewardedVideoOffer(string place) => new CustomEvent();

        public override IEvent RewardedVideoDownloaded(string place) => new CustomEvent();

        public override IEvent RewardedVideoCalled(string place) => new CustomEvent();

        public override IEvent RewardedVideoShow(int level, string place) => new RewardedVideoShow(level, place);

        public override IEvent RewardedVideoShowCompleted(int level, string place, bool isRewarded) => new RewardedAdsShowCompleted(place, isRewarded ? "success" : "skip");

        public override IEvent RewardedVideoClick(string place) { return new CustomEvent(); }

        public override IEvent RewardedVideoShowFail(string place, string msg) { return new CustomEvent(); }

        public override IEvent LevelLose(int level, int timeSpent, int loseCount) => new LevelLose(level, timeSpent);

        public override IEvent LevelStart(int level, int gold) => new LevelStart(level);

        public override IEvent LevelWin(int level, int timeSpent, int winCount) => new LevelWin(level, timeSpent);

        public override IEvent FirstWin(int level, int timeSpent) => new CustomEvent();

        public override IEvent LevelSkipped(int level, int timeSpent) => new LevelSkipped(level, timeSpent);

        public override IEvent EarnVirtualCurrency(string virtualCurrencyName, long value, string source) { return new CustomEvent(); }

        public override IEvent SpendVirtualCurrency(string virtualCurrencyName, long value, string itemName) { return new CustomEvent(); }

        public override IEvent TutorialCompletion(bool success, string tutorialId) { return new CustomEvent(); }

        public override IEvent EarnVirtualCurrency(string type, int amount) { return new CustomEvent(); }

        public override void ForceUpdateAllProperties() { }

        public override string LevelMaxProperty                   => "level_reach";
        public override string LastLevelProperty                  => "last_level";
        public override string LastAdsPlacementProperty           => "last_placement";
        public override string TotalInterstitialAdsProperty       => "total_interstitial_ads";
        public override string TotalRewardedAdsProperty           => "total_rewarded_ads";
        public override string TotalVirtualCurrencySpentProperty  => "total_spent";
        public override string TotalVirtualCurrencyEarnedProperty => "total_earn";
        public override string DaysPlayedProperty                 => "days_playing";
        

        public override AnalyticsEventCustomizationConfig AppsFlyerAnalyticsEventCustomizationConfig { get; set; } = new()
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
                { nameof(AdInterDownloaded), "af_inters_ad_eligible" },
                { nameof(AdsRewardClick), "af_rewarded_ad_eligible" },
                { nameof(AdsRewardedDownloaded), "af_rewarded_api_called" },
                { nameof(AdsRewardShow), "af_rewarded_displayed" },
                { nameof(AdsRewardComplete), "af_rewarded_ad_completed" },
            }
        };

        public override AnalyticsEventCustomizationConfig FireBaseAnalyticsEventCustomizationConfig { get; set; } = new();
    }
}
#endif