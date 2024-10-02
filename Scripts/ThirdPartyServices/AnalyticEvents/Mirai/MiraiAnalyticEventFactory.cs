#if MIRAI
namespace TheOneStudio.UITemplate.UITemplate.ThirdPartyServices.AnalyticEvents.Mirai
{
    using System;
    using System.Collections.Generic;
    using Core.AnalyticServices.CommonEvents;
    using Core.AnalyticServices.Data;
    using TheOneStudio.UITemplate.UITemplate.ThirdPartyServices.AnalyticEvents.CommonEvents;
    using UnityEngine.Scripting;

    [Preserve]
    public class MiraiAnalyticEventFactory : BaseAnalyticEventFactory
    {

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
                typeof(InterstitialAdClicked),
                typeof(InterstitialAdDisplayedFailed),
                typeof(InterstitialAdDownloaded),
                typeof(RewardedAdShowFail),
                typeof(RewardedAdOffer),
                typeof(RewardedAdLoaded),
                typeof(RewardedAdLoadClicked),
                typeof(LevelWin),
            },
            CustomEventKeys = new Dictionary<string, string>()
            {
                { nameof(BannerShown), "af_banner_shown" },
                { nameof(TutorialCompletion), "af_tutorial_completion" },
                { nameof(FirstWin), "af_level_achieved" },
                { nameof(InterstitialAdEligible), "af_inters_ad_eligible" },
                { nameof(InterstitialAdCalled), "af_inters_api_called" },
                { nameof(InterstitialAdDisplayed), "af_inters_displayed" },
                { nameof(RewardedAdEligible), "af_rewarded_ad_eligible" },
                { nameof(RewardedAdCalled), "af_rewarded_api_called" },
                { nameof(RewardedAdDisplayed), "af_rewarded_displayed" }
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
                { "Message", "errormsg" },
                { "worldId", "world_id" },
                { nameof(CommonEvents.LevelStart), "level_start" },
                { nameof(LevelEnd), "level_end" },
                { nameof(BannerShown), "ads_Banner_show" },
                { nameof(BannerLoadFail), "ads_Banner_fail" },
                { nameof(InterstitialAdDisplayed), "ads_Inter_show" },
                { nameof(InterstitialAdDisplayedFailed), "ads_Inter_fail" },
                { nameof(RewardedAdDisplayed), "ads_Reward_show" },
                { nameof(RewardedAdCompleted), "ads_Reward_complete" },
                { nameof(RewardedAdShowFail), "ads_Reward_fail" },
                { nameof(CommonEvents.AppOpenFullScreenContentFailed), "ads_Open_fail" },
                { nameof(CommonEvents.AppOpenFullScreenContentOpened), "ads_Open_show" },
                { "GameLaunched", "open_app" },
            }
        };

        public override IEvent LevelStart(int level, int gold, int totalLevelsPlayed, long timestamp, int gameModeId, int totalLevelsTypePlayed)
        {
            return
                new CustomEvent()
                {
                    EventName = "level_start",
                    EventProperties = new Dictionary<string, object>()
                    {
                        { "level", level },
                        { "gold", gold },
                        { "world_id", totalLevelsPlayed },
                        { "time_stamp", timestamp }
                    }
                };
        }

        public override IEvent LevelLose(int level, int timeSpent, int loseCount) { return new LevelEnd(level, "lose", 0, timeSpent, DateTimeOffset.UtcNow.ToUnixTimeSeconds()); }

        public override IEvent LevelWin(int level, int timeSpent, int winCount) { return new LevelEnd(level, "win", 0, timeSpent, DateTimeOffset.UtcNow.ToUnixTimeSeconds()); }

        public override IEvent LevelSkipped(int level, int timeSpent) { return new LevelEnd(level, "skip", 0, timeSpent, DateTimeOffset.UtcNow.ToUnixTimeSeconds()); }

        public override IEvent RewardedVideoShowFail(string place, string msg) { return new RewardedAdShowFail(place, msg); }
    }
}
#endif