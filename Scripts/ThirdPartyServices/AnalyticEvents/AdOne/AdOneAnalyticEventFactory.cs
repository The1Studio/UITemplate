#if ADONE

namespace TheOneStudio.UITemplate.UITemplate.Scripts.ThirdPartyServices.AnalyticEvents.AdOne
{
    using System;
    using System.Collections.Generic;
    using Core.AnalyticServices;
    using Core.AnalyticServices.CommonEvents;
    using Core.AnalyticServices.Data;
    using TheOneStudio.UITemplate.UITemplate.Models;
    using TheOneStudio.UITemplate.UITemplate.Models.Controllers;
    using TheOneStudio.UITemplate.UITemplate.Services;
    using TheOneStudio.UITemplate.UITemplate.ThirdPartyServices.AnalyticEvents;
    using GameFoundation.Signals;
    using UnityEngine.Scripting;

    public class AdOneAnalyticEventFactory : BaseAnalyticEventFactory
    {
        #region inject

        private readonly IInternetService              internetService;
        private readonly UITemplateLevelDataController uiTemplateLevelDataController;

        #endregion

        [Preserve]
        public AdOneAnalyticEventFactory(SignalBus signalBus, IAnalyticServices analyticServices, IInternetService internetService, UITemplateLevelDataController uiTemplateLevelDataController) : base(
            signalBus, analyticServices)
        {
            this.internetService               = internetService;
            this.uiTemplateLevelDataController = uiTemplateLevelDataController;
        }

        public override IEvent InterstitialShow(int level, string place) { return new AdInterShow(place); }

        public override IEvent InterstitialCalled(string place) { return new AdInterCalled(place); }

        public override IEvent RewardedVideoShow(int level, string place) { return new AdsRewardShow(place); }

        public override IEvent RewardedVideoCalled(string place) { return new AdsRewardedCalled(place); }

        public override IEvent LevelLose(int level, int timeSpent, int loseCount) { return new LevelFailed(level, timeSpent); }

        public override IEvent LevelStart(int level, int gold) { return new LevelStart(level, this.uiTemplateLevelDataController.GetLevelData(level).LevelStatus == LevelData.Status.Passed); }

        public override IEvent LevelWin(int level, int timeSpent, int winCount) { return new LevelPassed(level, timeSpent); }

        public override IEvent LevelSkipped(int level, int timeSpent) { return new LevelSkipped(level, timeSpent); }

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
                { nameof(AppOpenFullScreenContentOpened), "af_AOA" },
                { nameof(AdsRewardShow), "af_rewarded" }
            }
        };

        public override AnalyticsEventCustomizationConfig FireBaseAnalyticsEventCustomizationConfig { get; set; } = new()
        {
            IgnoreEvents = new HashSet<Type>(),
            CustomEventKeys = new Dictionary<string, string>()
            {
                { nameof(AppOpenFullScreenContentOpened), "af_AOA" },
                { nameof(AdInterShow), "af_inters" },
                { nameof(AdInterCalled), "inter_attempt" },
                { nameof(AdsRewardShow), "af_rewarded" },
                { nameof(AdsRewardedCalled), "reward_attempt" }
            }
        };
    }
}
#endif