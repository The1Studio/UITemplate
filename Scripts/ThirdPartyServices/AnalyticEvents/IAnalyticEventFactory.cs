namespace TheOneStudio.UITemplate.UITemplate.ThirdPartyServices.AnalyticEvents
{
    using Core.AnalyticServices.Data;

    public interface IAnalyticEventFactory
    {
        IEvent InterstitialShow(int           level, string place);
        IEvent InterstitialShowCompleted(int  level, string place);
        IEvent RewardedVideoShow(int          level, string place);
        IEvent RewardedVideoShowCompleted(int level, string place, bool isRewarded);
        IEvent LevelLose(int                  level, int    timeSpent);
        IEvent LevelStart(int                 level);
        IEvent LevelWin(int                   level, int timeSpent);
        IEvent LevelSkipped(int               level, int timeSpent);
        void   ForceUpdateAllProperties();
        string LevelMaxProperty { get; }
        string LastLevelProperty { get; }
        string LastAdsPlacementProperty { get; }
        string TotalInterstitialAdsProperty { get; }
        string TotalRewardedAdsProperty { get; }
    }
}