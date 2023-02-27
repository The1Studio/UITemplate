namespace TheOneStudio.UITemplate.UITemplate.ThirdPartyServices.AnalyticEvents.OneSoft
{
    using Core.AnalyticServices.Data;

    public class OneSoftAnalyticEventFactory : IAnalyticEventFactory
    {
        public IEvent InterstitialShow(int           level, string place)                  => new InterstitialShow(level, place);
        public IEvent InterstitialShowCompleted(int  level, string place)                  => new InterstitialShowCompleted(place);
        public IEvent RewardedVideoShow(int          level, string place)                  => new RewardedVideoShow(level, place);
        public IEvent RewardedVideoShowCompleted(int level, string place, bool isRewarded) => new RewardedAdsShowCompleted(place, isRewarded ? "success" : "skip");
        public IEvent LevelLose(int                  level, int    timeSpent) => new LevelLose(level, timeSpent);
        public IEvent LevelStart(int                 level)                => new LevelStart(level);
        public IEvent LevelWin(int                   level, int timeSpent) => new LevelWin(level, timeSpent);
        public IEvent LevelSkipped(int               level, int timeSpent) => new LevelSkipped(level, timeSpent);
        public void ForceUpdateAllProperties()
        {
        }
        public string LevelMaxProperty             => "level_max";
        public string LastLevelProperty            => "last_level";
        public string LastAdsPlacementProperty     => "last_placement";
        public string TotalInterstitialAdsProperty => "total_interstitial_ads";
        public string TotalRewardedAdsProperty     => "total_rewarded_ads";
    }
}