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

    public class AdOneAnalyticEventFactory : IAnalyticEventFactory
    {
        #region inject

        private readonly IInternetService              internetService;
        private readonly UITemplateLevelDataController uiTemplateLevelDataController;

        #endregion

        public AdOneAnalyticEventFactory(IInternetService internetService, UITemplateLevelDataController uiTemplateLevelDataController)
        {
            this.internetService               = internetService;
            this.uiTemplateLevelDataController = uiTemplateLevelDataController;
        }

        public IEvent InterstitialEligible(string place)               => new CustomEvent();
        public IEvent InterstitialShow(int        level, string place) { return new ShowInterstitialAds(this.internetService.IsInternetAvailable, place); }

        public IEvent InterstitialShowCompleted(int level, string place) { return new InterstitialAdsSuccess(place); }

        public IEvent InterstitialShowFail(string place, string msg) { return new CustomEvent(); }

        public IEvent InterstitialClick(string place) { return new CustomEvent(); }

        public IEvent InterstitialLoaded(string   place) { return new CustomEvent(); }
        public IEvent RewardedVideoEligible(string place) => new CustomEvent();

        public IEvent RewardedVideoOffer(string place) { return new CustomEvent(); }

        public IEvent RewardedVideoLoaded(string place) { return new CustomEvent(); }

        public IEvent RewardedVideoShow(int level, string place) { return new ShowRewardedAds(this.internetService.IsInternetAvailable, place); }

        public IEvent RewardedVideoShowCompleted(int level, string place, bool isRewarded) { return new RewardedAdsSuccess(place, isRewarded ? "success" : "skip"); }

        public IEvent RewardedVideoClick(string place) { return new CustomEvent(); }

        public IEvent RewardedVideoShowFail(string place, string msg) { return new CustomEvent(); }

        public IEvent LevelLose(int level, int timeSpent, int loseCount) { return new LevelFailed(level, timeSpent); }

        public IEvent LevelStart(int level, int gold) { return new LevelStart(level, this.uiTemplateLevelDataController.GetLevelData(level).LevelStatus == LevelData.Status.Passed); }

        public IEvent LevelWin(int level, int timeSpent, int winCount) { return new LevelPassed(level, timeSpent); }

        public IEvent FirstWin(int level, int timeSpent) { return new CustomEvent(); }

        public IEvent LevelSkipped(int level, int timeSpent) { return new LevelSkipped(level, timeSpent); }

        public IEvent EarnVirtualCurrency(string virtualCurrencyName, long value, string source) { return new CustomEvent(); }

        public IEvent SpendVirtualCurrency(string virtualCurrencyName, long value, string itemName) { return new CustomEvent(); }

        public void ForceUpdateAllProperties() { }

        public string LevelMaxProperty             => "level_max";
        public string LastLevelProperty            => "last_level";
        public string LastAdsPlacementProperty     => "last_placement";
        public string TotalInterstitialAdsProperty => "total_interstitial_ads";
        public string TotalRewardedAdsProperty     => "total_rewarded_ads";

        public AnalyticsEventCustomizationConfig AppsFlyerAnalyticsEventCustomizationConfig { get; set; } = new()
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

        public AnalyticsEventCustomizationConfig FireBaseAnalyticsEventCustomizationConfig { get; set; } = new()
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