namespace TheOneStudio.UITemplate.UITemplate.ThirdPartyServices.AnalyticEvents.Wido
{
    using Core.AnalyticServices.Data;
    using TheOneStudio.UITemplate.UITemplate.Models;
    using TheOneStudio.UITemplate.UITemplate.Models.Controllers;
    using TheOneStudio.UITemplate.UITemplate.Services;
    using TheOneStudio.UITemplate.UITemplate.ThirdPartyServices.AnalyticEvents.OneSoft;

    public class WidoAnalyticEventFactory : IAnalyticEventFactory
    {
        #region inject

        private readonly IInternetService              internetService;
        private readonly UITemplateLevelDataController uiTemplateLevelDataController;

        #endregion

        public WidoAnalyticEventFactory(IInternetService internetService, UITemplateLevelDataController uiTemplateLevelDataController)
        {
            this.internetService               = internetService;
            this.uiTemplateLevelDataController = uiTemplateLevelDataController;
        }

        public IEvent InterstitialShow(int           level, string place)                  { return new ShowInterstitialAds(this.internetService.IsInternetAvailable, place); }
        public IEvent InterstitialShowCompleted(int  level, string place)                  { return new InterstitialAdsSuccess(place); }
        public IEvent RewardedVideoShow(int          level, string place)                  { return new ShowRewardedAds(this.internetService.IsInternetAvailable, place); }
        public IEvent RewardedVideoShowCompleted(int level, string place, bool isRewarded) { return new RewardedAdsSuccess(place, isRewarded ? "success" : "skip"); }
        public IEvent LevelLose(int                  level, int    timeSpent) { return new LevelFailed(level, timeSpent); }
        public IEvent LevelStart(int                 level)                { return new LevelStart(level, this.uiTemplateLevelDataController.GetLevelData(level).LevelStatus == LevelData.Status.Passed); }
        public IEvent LevelWin(int                   level, int timeSpent) { return new LevelPassed(level, timeSpent); }

        public IEvent LevelSkipped(int level, int timeSpent) { return new LevelSkipped(level, timeSpent); }

        public void   ForceUpdateAllProperties()   { }
        
        public string LevelMaxProperty             => "level_max";
        public string LastLevelProperty            => "last_level";
        public string LastAdsPlacementProperty        => "last_placement";
        public string TotalInterstitialAdsProperty => "total_interstitial_ads";
        public string TotalRewardedAdsProperty     => "total_rewarded_ads";
    }
}