#if MIRAI
namespace TheOneStudio.UITemplate.UITemplate.ThirdPartyServices.AnalyticEvents.Mirai
{
    using System;
    using System.Collections.Generic;
    using Core.AnalyticServices;
    using Core.AnalyticServices.CommonEvents;
    using Core.AnalyticServices.Data;
    using TheOneStudio.UITemplate.UITemplate.Scripts.ThirdPartyServices.AnalyticEvents.ABI;
    using TheOneStudio.UITemplate.UITemplate.ThirdPartyServices.AnalyticEvents.ABI;
    using GameFoundation.Signals;
    using TheOneStudio.UITemplate.UITemplate.ThirdPartyServices.AnalyticEvents.CommonEvents;
    using LevelStart = TheOneStudio.UITemplate.UITemplate.ThirdPartyServices.AnalyticEvents.CommonEvents.LevelStart;
    using UnityEngine.Scripting;

    public class MiraiAnalyticEventFactory : BaseAnalyticEventFactory
    {
        [Preserve]
        public MiraiAnalyticEventFactory(SignalBus signalBus, IAnalyticServices analyticServices) : base(signalBus, analyticServices)
        {
        }

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
                typeof(LevelComplete),
            },
            CustomEventKeys = new Dictionary<string, string>()
            {
                { nameof(BannerShown), "af_banner_shown" },
                { nameof(GameTutorialCompletion), "af_tutorial_completion" },
                { nameof(LevelAchieved), "af_level_achieved" },
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
                { nameof(AdsRewardFail), "ads_Reward_fail" },
                { nameof(CommonEvents.AppOpenFullScreenContentFailed), "ads_Open_fail" },
                { nameof(CommonEvents.AppOpenFullScreenContentOpened), "ads_Open_show" },
                { "GameLaunched", "open_app" },
            }
        };

        public override IEvent LevelStart(int level, int gold) { return new LevelStart(level, gold, 0, DateTimeOffset.UtcNow.ToUnixTimeSeconds()); }

        public override IEvent LevelLose(int level, int timeSpent, int loseCount) { return new LevelEnd(level, "lose", 0, timeSpent, DateTimeOffset.UtcNow.ToUnixTimeSeconds()); }

        public override IEvent LevelWin(int level, int timeSpent, int winCount) { return new LevelEnd(level, "win", 0, timeSpent, DateTimeOffset.UtcNow.ToUnixTimeSeconds()); }

        public override IEvent LevelSkipped(int level, int timeSpent) { return new LevelEnd(level, "skip", 0, timeSpent, DateTimeOffset.UtcNow.ToUnixTimeSeconds()); }

        public override IEvent RewardedVideoShowFail(string place, string msg) { return new AdsRewardFail(place, msg); }
    }
}
#endif