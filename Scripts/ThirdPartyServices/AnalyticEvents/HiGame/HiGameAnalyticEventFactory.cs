#if HIGAME
namespace TheOneStudio.UITemplate.UITemplate.ThirdPartyServices.AnalyticEvents.HiGame
{
    using System;
    using System.Collections.Generic;
    using Core.AnalyticServices;
    using Core.AnalyticServices.CommonEvents;
    using Core.AnalyticServices.Data;
    using GameFoundation.Signals;
    using TheOneStudio.UITemplate.UITemplate.ThirdPartyServices.AnalyticEvents.CommonEvents;
    using UnityEngine.Scripting;

    public class HiGameAnalyticEventFactory : BaseAnalyticEventFactory
    {
        [Preserve]
        public HiGameAnalyticEventFactory(SignalBus signalBus, IAnalyticServices analyticServices)
            : base(signalBus, analyticServices)
        {
        }

        public override string RetentionDayProperty              => "retent_type";
        public override string DaysPlayedProperty                => "days_played";
        public override string TotalVirtualCurrencySpentProperty => "total_spent";
        public override string LevelMaxProperty                  => "level";
        public override string LastLevelProperty                 => "last_level";

        public override AnalyticsEventCustomizationConfig AppsFlyerAnalyticsEventCustomizationConfig { get; set; } = new()
        {
            IgnoreEvents = new HashSet<Type>()
            {
            },
            CustomEventKeys = new Dictionary<string, string>()
            {
                { nameof(CommonEvents.TutorialCompletion), "af_tutorial_completion" },
                { "success", "af_success" },
                { "tutorialId", "af_tutorial_id" },
                
                { nameof(LevelEnd), "af_level_achieved" },
                { "level", "af_level" },
                
                { nameof(InterstitialAdCalled), "af_inters_call_show" },
                { nameof(InterstitialAdEligible), "af_inters_passed_capping_time" },
                { nameof(InterstitialAdDownloaded), "af_inters_available" },
                { nameof(InterstitialAdDisplayed), "af_inters_displayed" },
                { nameof(InterstitialAdLoadFailed), "af_inters_load_fail" },
                
                { nameof(RewardedAdEligible), "af_rewarded_ad_eligible" },
                { nameof(RewardedAdCalled), "af_rewarded_api_called" },
                { nameof(RewardedAdDisplayed), "af_rewarded_displayed" },
                { nameof(RewardedAdCompleted), "af_rewarded_ad_completed" },
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
                
                { nameof(LevelEnd), "level_complete" },
                { "timePlay", "timeplayed" },
                { nameof(CommonEvents.LevelStart), "level_start" },
                { "gold", "current_gold" },

                { nameof(RewardedAdEligible), "ads_reward_offer" },
                { nameof(RewardedAdLoadClicked), "ads_reward_click" },
                { nameof(RewardedAdDisplayed), "ads_reward_show" },
                { nameof(RewardedAdShowFail), "ads_reward_fail" },
                { nameof(RewardedAdCompleted), "ads_reward_complete" },
                
                { nameof(InterstitialAdDisplayedFailed), "ad_inter_fail" },
                { nameof(InterstitialAdDownloaded), "ad_inter_load" },
                { nameof(InterstitialAdClicked), "ad_inter_click" },
                { nameof(InterstitialAdDisplayed), "ad_inter_show" },
            }
        };

        public override IEvent LevelLose(int level, int timeSpent, int loseCount)
        {
            this.analyticServices.Track(new CustomEvent()
            {
                EventName = "level_fail",
                EventProperties = new Dictionary<string, object>()
                {
                    { "level", level },
                    { "time_spent", timeSpent },
                    { "failcount", loseCount },
                    { "timestamp", DateTimeOffset.UtcNow.ToUnixTimeSeconds() }
                }
            });
            return new LevelEnd(level, "lose", 0, timeSpent, DateTimeOffset.UtcNow.ToUnixTimeSeconds());
        }

        public override IEvent LevelWin(int level, int timeSpent, int winCount)
        {
            return new LevelEnd(level, "win", 0, timeSpent, DateTimeOffset.UtcNow.ToUnixTimeSeconds());
        }

        public override IEvent LevelSkipped(int level, int timeSpent)
        {
            return new LevelEnd(level, "skip", 0, timeSpent, DateTimeOffset.UtcNow.ToUnixTimeSeconds());
        }
    }
}
#endif