#if BRAVESTARS
namespace TheOneStudio.UITemplate.UITemplate.ThirdPartyServices.AnalyticEvents.BraveStars
{
    using System;
    using System.Collections.Generic;
    using Core.AnalyticServices.CommonEvents;
    using Core.AnalyticServices.Data;
    using TheOneStudio.UITemplate.UITemplate.ThirdPartyServices.AnalyticEvents.CommonEvents;
    using UnityEngine.Scripting;

    public class BraveStarsAnalyticEventFactory : BaseAnalyticEventFactory
    {
        private const int MAX_LEVEL_FIRE_COMPLETED_LEVEL_AF_EVENT = 20;
        private const int MAX_TIME_FIRE_INTER_DISPLAYED_AF_EVENT = 20;

        [Preserve]
        public BraveStarsAnalyticEventFactory(SignalBus signalBus, IAnalyticServices analyticServices) : base(signalBus, analyticServices)
        {
            for (var i = 1; i <= MAX_LEVEL_FIRE_COMPLETED_LEVEL_AF_EVENT; i++)
            {
                this.AppsFlyerAnalyticsEventCustomizationConfig.IncludeEvents.Add($"completed_level_{i}");
            }
            for (var i = 1; i <= MAX_TIME_FIRE_INTER_DISPLAYED_AF_EVENT; i++)
            {
                this.AppsFlyerAnalyticsEventCustomizationConfig.IncludeEvents.Add($"af_inters_displayed_{i}_times");
            }
        }

        public override IEvent LevelWin(int level, int timeSpent, int winCount) => new CustomEvent { EventName = $"win_level_{level}" };

        public override IEvent LevelStart(int level, int gold, int totalLevelsPlayed, long? timestamp, int gameModeId, int totalLevelsTypePlayed) => new CustomEvent { EventName = $"start_level_{level}" };

        public override AnalyticsEventCustomizationConfig AppsFlyerAnalyticsEventCustomizationConfig { get; set; } = new()
        {
            IncludeEvents = new HashSet<string>()
                            {
                                "af_banner_shown",
                                "af_tutorial_completion",
                                "af_achieved_level",
                                "af_inters_ad_eligible",
                                "af_inters_api_called",
                                "af_inters_displayed",
                                "af_rewarded_ad_eligible",
                                "af_rewarded_api_called",
                                "af_rewarded_ad_displayed",
                            },
            CustomEventKeys = new Dictionary<string, string>()
            {
                { nameof(BannerShown), "af_banner_shown" },
                { nameof(TutorialCompletion), "af_tutorial_completion" },
                { nameof(AchievedLevel), "af_achieved_level" },
                { nameof(InterstitialAdEligible), "af_inters_ad_eligible" },
                { nameof(InterstitialAdDownloaded), "af_inters_api_called" },
                { nameof(InterstitialAdDisplayed), "af_inters_displayed" },
                { nameof(RewardedAdEligible), "af_rewarded_ad_eligible" },
                { nameof(RewardedAdLoaded), "af_rewarded_api_called" },
                { nameof(RewardedAdDisplayed), "af_rewarded_ad_displayed" }
            }
        };

        public override AnalyticsEventCustomizationConfig FireBaseAnalyticsEventCustomizationConfig { get; set; } = new()
        {
            IgnoreEvents = new HashSet<Type>(),
            CustomEventKeys = new Dictionary<string, string>()
            {
                { nameof(AdsRevenueEvent), "ad_impression_abi" },
                { "AdsRevenueSourceId", "ad_platform" },
                { "AdNetwork", "ad_source" },
                { "AdUnit", "ad_unit_name" },
                { "AdFormat", "ad_format" },
                { "Placement", "placement" },
                { "Currency", "currency" },
                { "Revenue", "value" },
                { nameof(FirstWin), "checkpoint" },
            }
        };
    }
}
#endif