#if ROCKET

namespace TheOneStudio.UITemplate.UITemplate.ThirdPartyServices.AnalyticEvents.Rocket
{
    using System;
    using System.Collections.Generic;
    using Core.AnalyticServices;
    using Core.AnalyticServices.CommonEvents;
    using Core.AnalyticServices.Data;
    using TheOneStudio.UITemplate.UITemplate.Scripts.ThirdPartyServices.AnalyticEvents.ABI;
    using TheOneStudio.UITemplate.UITemplate.ThirdPartyServices.AnalyticEvents.ABI;
    using GameFoundation.Signals;
    using UnityEngine.Scripting;

    public class RocketAnalyticEventFactory : BaseAnalyticEventFactory
    {
        [Preserve]
        public RocketAnalyticEventFactory(SignalBus signalBus, IAnalyticServices analyticServices) : base(signalBus, analyticServices) { }

        public override IEvent InterstitialShow(int level, string place) => new InterstitialShow(level, place);

        public override IEvent InterstitialShowCompleted(int level, string place) => new InterstitialShowCompleted(place);

        public override IEvent RewardedVideoShow(int level, string place) => new RewardedVideoShow(level, place);

        public override IEvent RewardedVideoShowCompleted(int level, string place, bool isRewarded) => new RewardedAdsShowCompleted(place, isRewarded ? "success" : "skip");

        public override IEvent LevelLose(int level, int timeSpent, int loseCount) => new LevelLose(level, timeSpent);

        public override IEvent LevelStart(int level, int gold) => new LevelStart(level);

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