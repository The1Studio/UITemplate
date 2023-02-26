namespace TheOneStudio.UITemplate.UITemplate.ThirdPartyServices.AnalyticEvents.Wido
{
    using Core.AnalyticServices.Data;
    using TheOneStudio.UITemplate.UITemplate.Models;
    using TheOneStudio.UITemplate.UITemplate.Services;
    using TheOneStudio.UITemplate.UITemplate.ThirdPartyServices.AnalyticEvents.OneSoft;

    public class WidoAnalyticEventFactory : IAnalyticEventFactory
    {
        #region inject

        private readonly IInternetService        internetService;
        private readonly UITemplateUserLevelData uiTemplateUserLevelData;

        #endregion
        
        public WidoAnalyticEventFactory(IInternetService internetService, UITemplateUserLevelData uiTemplateUserLevelData)
        {
            this.internetService         = internetService;
            this.uiTemplateUserLevelData = uiTemplateUserLevelData;
        }
        
        public IEvent InterstitialShow(int level, string place)
        {
            return new ShowInterstitialAds(this.internetService.IsInternetAvailable, place);
        }
        public IEvent InterstitialShowCompleted(int level, string place)
        {
            return new InterstitialAdsSuccess(place);
        }
        public IEvent RewardedVideoShow(int level, string place)
        {
            return new ShowRewardedAds(this.internetService.IsInternetAvailable, place);
        }
        public IEvent RewardedVideoShowCompleted(int level, string place, bool isRewarded)
        {
            return new RewardedAdsSuccess(place, isRewarded ? "success" : "skip");
        }
        public IEvent LevelLose(int level, int timeSpent)
        {
            return new LevelFailed(level, timeSpent);
        }
        public IEvent LevelStart(int level)
        {
            return new LevelStart(level, this.uiTemplateUserLevelData.GetLevelData(level).LevelStatus == LevelData.Status.Passed);
        }
        public IEvent LevelWin(int level, int timeSpent)
        {
            return new LevelPassed(level, timeSpent);
        }
        
        public IEvent LevelSkipped(int level, int timeSpent)
        {
            return new LevelSkipped(level, timeSpent);
        }
        
        public void ForceUpdateAllProperties()
        {
        }
    }
}