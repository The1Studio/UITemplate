#if FALCON
namespace TheOneStudio.UITemplate.UITemplate.ThirdPartyServices.AnalyticEvents.Falcon
{
    using System;
    using System.Collections.Generic;
    using Core.AnalyticServices;
    using Core.AnalyticServices.CommonEvents;
    using Core.AnalyticServices.Data;
    using TheOneStudio.UITemplate.UITemplate.Scripts.ThirdPartyServices.AnalyticEvents.ABI;
    using TheOneStudio.UITemplate.UITemplate.Scripts.ThirdPartyServices.AnalyticEvents.AdOne;
    using TheOneStudio.UITemplate.UITemplate.ThirdPartyServices.AnalyticEvents.ABI;
    using Zenject;
    using AdInterCalled = TheOneStudio.UITemplate.UITemplate.Scripts.ThirdPartyServices.AnalyticEvents.ABI.AdInterCalled;
    using AdInterClick = TheOneStudio.UITemplate.UITemplate.Scripts.ThirdPartyServices.AnalyticEvents.ABI.AdInterClick;
    using AdInterFail = TheOneStudio.UITemplate.UITemplate.Scripts.ThirdPartyServices.AnalyticEvents.ABI.AdInterFail;
    using AdInterLoad = TheOneStudio.UITemplate.UITemplate.Scripts.ThirdPartyServices.AnalyticEvents.ABI.AdInterLoad;
    using AdInterShow = TheOneStudio.UITemplate.UITemplate.Scripts.ThirdPartyServices.AnalyticEvents.ABI.AdInterShow;
    using AdsRewardClick = TheOneStudio.UITemplate.UITemplate.Scripts.ThirdPartyServices.AnalyticEvents.ABI.AdsRewardClick;
    using AdsRewardComplete = TheOneStudio.UITemplate.UITemplate.Scripts.ThirdPartyServices.AnalyticEvents.ABI.AdsRewardComplete;
    using AdsRewardedCalled = TheOneStudio.UITemplate.UITemplate.Scripts.ThirdPartyServices.AnalyticEvents.ABI.AdsRewardedCalled;
    using AdsRewardFail = TheOneStudio.UITemplate.UITemplate.Scripts.ThirdPartyServices.AnalyticEvents.ABI.AdsRewardFail;
    using AdsRewardOffer = TheOneStudio.UITemplate.UITemplate.Scripts.ThirdPartyServices.AnalyticEvents.ABI.AdsRewardOffer;
    using AdsRewardShow = TheOneStudio.UITemplate.UITemplate.Scripts.ThirdPartyServices.AnalyticEvents.ABI.AdsRewardShow;

    public class FalconAnalyticEventFactory : BaseAnalyticEventFactory
    {
        public FalconAnalyticEventFactory(SignalBus signalBus, IAnalyticServices analyticServices) : base(signalBus, analyticServices) { }

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
                { nameof(AdInterCalled), "af_inters_show " },
                { nameof(AdInterShow), "af_inters_displayed" },
                { nameof(AdsRewardEligible), "af_rewarded_ad_eligible" },
                { nameof(AdsRewardedCalled), "af_rewarded_api_called" },
                { nameof(AdsRewardShow), "af_rewarded_show" },
                { nameof(AdsRewardComplete), "af_rewarded_displayed" },
                { nameof(CommonEvents.LevelStart), "af_level_start" },
                { nameof(CommonEvents.LevelWin), "af_level_achieved" },
                { nameof(CommonEvents.LevelLose), "af_level_fail" },
            }
        };

        public override AnalyticsEventCustomizationConfig FireBaseAnalyticsEventCustomizationConfig { get; set; } = new()
        {
            IgnoreEvents = new HashSet<Type>(),
            CustomEventKeys = new Dictionary<string, string>()
            {
                { nameof(AdsRevenueEvent), "ad_impression_falcon" },
                { "AdsRevenueSourceId", "ad_platform" },
                { "AdNetwork", "ad_source" },
                { "AdUnit", "ad_unit_name" },
                { "AdFormat", "ad_format" },
                { "Placement", "placement" },
                { "Currency", "currency" },
                { "Revenue", "value" },
                { nameof(LevelAchieved), "checkpoint" },
                { nameof(AdInterLoad), "ad_inter_load_success" },
                { nameof(AdInterFail), "ad_inter_load_fail" },
                { nameof(AdInterShow), "ad_inter_show" },
                { nameof(AdInterClick), "ad_inter_click" },
                { nameof(AdsRewardedLoaded), "ads_reward_load" },
                { nameof(AdsRewardClick), "ads_reward_click" },
                { nameof(AdsRewardShow), "ads_reward_show_success" },
                { nameof(AdsRewardFail), "ads_reward_show_fail" },
                { nameof(AdsRewardComplete), "ads_reward_complete" },
            }
        };
    }
}
#endif