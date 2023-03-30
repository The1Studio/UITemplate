#if ADONE
namespace TheOneStudio.UITemplate.UITemplate.Scripts.ThirdPartyServices.AnalyticEvents.AdOne
{
    using System;
    using System.Collections.Generic;
    using Core.AnalyticServices.CommonEvents;
    using Core.AnalyticServices.Data;
    using TheOneStudio.UITemplate.UITemplate.Models;
    using TheOneStudio.UITemplate.UITemplate.Models.Controllers;
    using TheOneStudio.UITemplate.UITemplate.Services;
    using TheOneStudio.UITemplate.UITemplate.ThirdPartyServices.AnalyticEvents;

    public class AdOneAnalyticEventFactory : BaseAnalyticEventFactory
    {
        #region inject

        private readonly IInternetService              internetService;
        private readonly UITemplateLevelDataController uiTemplateLevelDataController;

        #endregion

        public AdOneAnalyticEventFactory(IInternetService internetService, UITemplateLevelDataController uiTemplateLevelDataController)
        {
            this.internetService = internetService;
            this.uiTemplateLevelDataController = uiTemplateLevelDataController;
        }

        public override IEvent InterstitialEligible(string place) => new CustomEvent();

        public override IEvent InterstitialShow(int level, string place) { return new ShowInterstitialAds(this.internetService.IsInternetAvailable, place); }

        public override IEvent InterstitialShowCompleted(int level, string place) { return new InterstitialAdsSuccess(place); }

        public override IEvent InterstitialShowFail(string place, string msg) { return new CustomEvent(); }

        public override IEvent InterstitialClick(string place) { return new CustomEvent(); }

        public override IEvent InterstitialDownloaded(string place) { return new CustomEvent(); }

        public override IEvent InterstitialCalled(string place) { return new CustomEvent(); }

        public override IEvent RewardedVideoEligible(string place) => new CustomEvent();

        public override IEvent RewardedVideoOffer(string place) { return new CustomEvent(); }

        public override IEvent RewardedVideoDownloaded(string place) { return new CustomEvent(); }

        public override IEvent RewardedVideoCalled(string place) { return new CustomEvent(); }

        public override IEvent RewardedVideoShow(int level, string place) { return new ShowRewardedAds(this.internetService.IsInternetAvailable, place); }

        public override IEvent RewardedVideoShowCompleted(int level, string place, bool isRewarded) { return new RewardedAdsSuccess(place, isRewarded ? "success" : "skip"); }

        public override IEvent RewardedVideoClick(string place) { return new CustomEvent(); }

        public override IEvent RewardedVideoShowFail(string place, string msg) { return new CustomEvent(); }

        public override IEvent LevelLose(int level, int timeSpent, int loseCount) { return new LevelFailed(level, timeSpent); }

        public override IEvent LevelStart(int level, int gold) { return new LevelStart(level, this.uiTemplateLevelDataController.GetLevelData(level).LevelStatus == LevelData.Status.Passed); }

        public override IEvent LevelWin(int level, int timeSpent, int winCount) { return new LevelPassed(level, timeSpent); }

        public override IEvent FirstWin(int level, int timeSpent) { return new CustomEvent(); }

        public override IEvent LevelSkipped(int level, int timeSpent) { return new LevelSkipped(level, timeSpent); }

        public override IEvent EarnVirtualCurrency(string virtualCurrencyName, long value, string source) { return new CustomEvent(); }

        public override IEvent SpendVirtualCurrency(string virtualCurrencyName, long value, string itemName) { return new CustomEvent(); }

        public override IEvent TutorialCompletion(bool success, string tutorialId) { return new CustomEvent(); }

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
            },
            CustomEventKeys = new Dictionary<string, string>()
            {
                { nameof(AdInterShow), "af_inters" },
                { nameof(AdsRewardShow), "af_reward" }
            }
        };

        public override AnalyticsEventCustomizationConfig FireBaseAnalyticsEventCustomizationConfig { get; set; } = new()
        {
            IgnoreEvents = new HashSet<Type>(),
            CustomEventKeys = new Dictionary<string, string>()
            {
                { nameof(InterstitialAdsSuccess), "af_inters" },
                { nameof(AdsRewardShow), "reward_attempt" },
                { nameof(AdInterShow), "inter_attempt" },
                { nameof(AdsRewardComplete), "af_reward" }
            }
        };
    }
}
#endif