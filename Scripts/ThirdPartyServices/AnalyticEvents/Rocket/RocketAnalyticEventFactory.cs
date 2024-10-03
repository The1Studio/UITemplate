#if ROCKET
namespace TheOneStudio.UITemplate.UITemplate.ThirdPartyServices.AnalyticEvents.Rocket
{
    using System;
    using System.Collections.Generic;
    using Core.AnalyticServices.CommonEvents;
    using Core.AnalyticServices.Data;
    using TheOneStudio.UITemplate.UITemplate.ThirdPartyServices.AnalyticEvents.CommonEvents;
    using UnityEngine.Scripting;

    [Preserve]
    public class RocketAnalyticEventFactory : BaseAnalyticEventFactory
    {
        public override IEvent InterstitialShow(int level, string place) => new InterstitialShow(level, place);

        public override IEvent InterstitialShowCompleted(int level, string place) => new InterstitialShowCompleted(place);

        public override IEvent RewardedVideoShow(int level, string place) => new RewardedVideoShow(level, place);

        public override IEvent RewardedVideoShowCompleted(int level, string place, bool isRewarded) => new RewardedAdsShowCompleted(place, isRewarded ? "success" : "skip");

        public override IEvent LevelLose(int level, int timeSpent, int loseCount) => new LevelLose(level, timeSpent);

        public override IEvent LevelStart(int level, int gold, int totalLevelsPlayed, long timeBetweenLastEvent, int gameModeId, int totalLevelsTypePlayed) => new LevelStart(level);

        public override IEvent LevelWin(int level, int timeSpent, int winCount) => new LevelWin(level, timeSpent);

        public override IEvent LevelSkipped(int level, int timeSpent) => new LevelSkipped(level, timeSpent);

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
                typeof(InterstitialAdClicked),
                typeof(InterstitialAdDisplayedFailed),
                typeof(RewardedAdShowFail),
                typeof(RewardedAdOffer),
            },
            CustomEventKeys = new Dictionary<string, string>()
            {
                { nameof(BannerShown), "af_banner_shown" },
                { nameof(LevelWin), "af_level_achieved" },
                { nameof(InterstitialCalled), "af_inters_api_called" },
                { nameof(InterstitialAdDisplayed), "af_inters_displayed" },
                { nameof(InterstitialAdEligible), "af_inters_ad_eligible" },
                { nameof(RewardedAdEligible), "af_rewarded_ad_eligible" },
                { nameof(RewardedAdCalled), "af_rewarded_api_called" },
                { nameof(RewardedAdDisplayed), "af_rewarded_displayed" },
                { nameof(RewardedAdCompleted), "af_rewarded_ad_completed" },
            }
        };

        public override AnalyticsEventCustomizationConfig FireBaseAnalyticsEventCustomizationConfig { get; set; } = new();
    }
}
#endif