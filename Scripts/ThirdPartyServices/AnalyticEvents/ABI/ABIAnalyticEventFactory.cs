#if ABI
namespace TheOneStudio.UITemplate.UITemplate.Scripts.ThirdPartyServices.AnalyticEvents.ABI
{
    using System;
    using System.Collections.Generic;
    using Core.AnalyticServices;
    using Core.AnalyticServices.CommonEvents;
    using Core.AnalyticServices.Data;
    using TheOneStudio.UITemplate.UITemplate.ThirdPartyServices.AnalyticEvents;
    using TheOneStudio.UITemplate.UITemplate.ThirdPartyServices.AnalyticEvents.ABI;
    using GameFoundation.Signals;
    using UnityEngine.Scripting;

    public class ABIAnalyticEventFactory : BaseAnalyticEventFactory
    {
        private readonly SignalBus         signalBus;
        private readonly IAnalyticServices analyticEvents;

        [Preserve]
        public ABIAnalyticEventFactory(SignalBus signalBus, IAnalyticServices analyticServices) : base(signalBus, analyticServices) { }
        public override IEvent InterstitialEligible(string place) => new AdsIntersEligible(place);

        public override IEvent InterstitialShow(int level, string place) => new AdInterShow(place);

        public override IEvent InterstitialShowFail(string place, string msg) => new AdInterFail(msg);

        public override IEvent InterstitialClick(string place) => new AdInterClick(place);

        public override IEvent InterstitialDownloaded(string place, long loadingMilis) => new AdInterDownloaded(place, loadingMilis);

        public override IEvent InterstitialCalled(string place) => new AdInterCalled();

        public override IEvent RewardedVideoEligible(string place) => new AdsRewardEligible(place);

        public override IEvent RewardedVideoOffer(string place) => new AdsRewardOffer(place);

        public override IEvent RewardedVideoDownloaded(string place, long loadingMilis) => new AdsRewardedDownloaded(place, loadingMilis);

        public override IEvent RewardedVideoCalled(string place) => new AdsRewardedCalled();

        public override IEvent RewardedVideoShow(int level, string place) => new AdsRewardShow(place);

        public override IEvent RewardedVideoClick(string place) => new AdsRewardClick(place);

        public override IEvent RewardedVideoShowFail(string place, string msg) => new AdsRewardFail(place, msg);

        public override IEvent LevelLose(int level, int timeSpent, int loseCount) => new LevelFail(level, loseCount);

        public override IEvent LevelStart(int level, int gold) => new LevelStart(level, gold);

        public override IEvent LevelWin(int level, int timeSpent, int winCount) => new LevelComplete(level, timeSpent);

        public override IEvent FirstWin(int level, int timeSpent) => new LevelAchieved(level, timeSpent);

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
                typeof(AdsRewardFail),
                typeof(AdsRewardOffer),
                typeof(AdsRewardClick),
                typeof(LevelComplete),
            },
            CustomEventKeys = new Dictionary<string, string>()
            {
                { nameof(BannerShown), "af_banner_shown" },
                { nameof(GameTutorialCompletion), "af_tutorial_completion" },
                { nameof(LevelAchieved), "af_level_achieved" },
                { nameof(AdsIntersEligible), "af_inters_logicgame" },
                { nameof(AdInterDownloaded), "af_inters_successfullyloaded" },
                { nameof(AdInterCalled), "af_inters_api_called" },
                { nameof(AdInterShow), "af_inters_displayed" },
                { nameof(AdsRewardEligible), "af_rewarded_logicgame" },
                { nameof(AdsRewardedDownloaded), "af_rewarded_successfullyloaded" },
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
                { nameof(LevelAchieved), "checkpoint" },
            }
        };
    }
}
#endif