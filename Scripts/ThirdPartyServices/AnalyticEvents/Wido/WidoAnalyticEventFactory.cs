#if WIDO
namespace TheOneStudio.UITemplate.UITemplate.Scripts.ThirdPartyServices.AnalyticEvents.Wido
{
    using Core.AnalyticServices.CommonEvents;
    using Core.AnalyticServices.Data;
    using TheOneStudio.UITemplate.UITemplate.Models;
    using TheOneStudio.UITemplate.UITemplate.Models.Controllers;
    using TheOneStudio.UITemplate.UITemplate.Services;
    using TheOneStudio.UITemplate.UITemplate.ThirdPartyServices.AnalyticEvents;
    using TheOneStudio.UITemplate.UITemplate.ThirdPartyServices.AnalyticEvents.CommonEvents;
    using TheOneStudio.UITemplate.UITemplate.ThirdPartyServices.AnalyticEvents.Wido;
    using UnityEngine.Scripting;
    using LevelStart = TheOneStudio.UITemplate.UITemplate.ThirdPartyServices.AnalyticEvents.Wido.LevelStart;

    public class WidoAnalyticEventFactory : BaseAnalyticEventFactory
    {
        #region inject

        private readonly IInternetService              internetService;
        private readonly UITemplateLevelDataController uiTemplateLevelDataController;

        [Preserve]
        public WidoAnalyticEventFactory(IInternetService internetService, UITemplateLevelDataController uiTemplateLevelDataController)
        {
            this.internetService = internetService;
            this.uiTemplateLevelDataController = uiTemplateLevelDataController;
        }

        #endregion

        public override IEvent InterstitialShow(int level, string place) { return new ShowInterstitialAds(this.internetService.IsInternetAvailable, place); }

        public override IEvent InterstitialShowCompleted(int level, string place) { return new InterstitialAdsSuccess(place); }

        public override IEvent RewardedVideoOffer(string place) { return new RewardedAdOffer(place); }


        public override IEvent RewardedVideoCalled(string place) { return new RewardedAdCalled(place); }

        public override IEvent RewardedVideoShow(int level, string place) { return new ShowRewardedAds(this.internetService.IsInternetAvailable, place); }

        public override IEvent RewardedVideoShowCompleted(int level, string place, bool isRewarded) { return new RewardedAdsSuccess(place, isRewarded ? "success" : "skip"); }

        public override IEvent LevelLose(int level, int timeSpent, int loseCount) { return new LevelFailed(level, timeSpent); }

        public override IEvent LevelStart(int level, int gold, int totalLevelsPlayed, long? timestamp, int gameModeId, int totalLevelsTypePlayed) { return new LevelStart(level, this.uiTemplateLevelDataController.GetLevelData(level).LevelStatus == LevelData.Status.Passed); }

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
            IgnoreEvents = new()
            {
                typeof(GameStarted),
                typeof(InterstitialAdClicked),
                typeof(InterstitialAdDisplayedFailed),
                typeof(RewardedAdShowFail),
                typeof(RewardedAdOffer),
            },
            CustomEventKeys = new()
            {
                { nameof(BannerShown), "af_banner_shown" },
                { nameof(LevelWin), "af_level_achieved" },
                { nameof(InterstitialAdCalled), "af_inters_api_called" },
                { nameof(InterstitialAdDisplayed), "af_inters_displayed" },
                { nameof(InterstitialAdEligible), "af_inters_ad_eligible" },
                { nameof(RewardedAdEligible), "af_rewarded_ad_eligible" },
                { nameof(RewardedAdCalled), "af_rewarded_api_called" },
                { nameof(RewardedAdDisplayed), "af_rewarded_displayed" },
                { nameof(RewardedAdCompleted), "af_rewarded_ad_completed" },
                {nameof(RewardedAdEligible), "af_ads_reward_eligible"}
            }
        };

        public override AnalyticsEventCustomizationConfig FireBaseAnalyticsEventCustomizationConfig { get; set; } = new()
        {
            CustomEventKeys = new()
            {
                { nameof(AppOpenFullScreenContentOpened), "aoa_show_success" },
                { nameof(ShowInterstitialAds), "inter_show_success" },
                { nameof(RewardedAdsSuccess), "rewarded_show_success" },
                { nameof(RewardedAdEligible), "ads_reward_eligible" },
                { nameof(RewardedAdOffer), "ads_reward_offer" },
                { nameof(RewardedAdCalled), "ads_reward_called" },
                { nameof(RewardedAdLoaded), "ads_rewarded_downloaded" }
            }
        };
    }
}
#endif