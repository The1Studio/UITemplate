namespace TheOneStudio.UITemplate.UITemplate.ThirdPartyServices.AnalyticEvents
{
    using Core.AnalyticServices.Data;

    public interface IAnalyticEventFactory
    {
        IEvent InterstitialShow(int level, string place);

        IEvent InterstitialShowCompleted(int level, string place);

        IEvent InterstitialShowFail(string place, string msg);

        IEvent InterstitialClick(string place);
        IEvent InterstitialRequest(string place);

        IEvent RewardedVideoOffer(string place);
        IEvent RewardedVideoLoaded(string place);

        IEvent RewardedVideoShow(int level, string place);

        IEvent RewardedVideoShowCompleted(int level, string place, bool isRewarded);

        IEvent RewardedVideoClick(string place);

        IEvent RewardedVideoShowFail(string place, string msg);

        IEvent LevelLose(int level, int timeSpent, int loseCount);

        IEvent LevelStart(int level, int gold);

        IEvent LevelWin(int level, int timeSpent, int winCount);

        IEvent FirstWin(int level, int timeSpent);

        IEvent LevelSkipped(int level, int timeSpent);

        IEvent EarnVirtualCurrency(string virtualCurrencyName, long value, string source);

        IEvent SpendVirtualCurrency(string virtualCurrencyName, long value, string itemName);

        void ForceUpdateAllProperties();

        string LevelMaxProperty             { get; }
        string LastLevelProperty            { get; }
        string LastAdsPlacementProperty     { get; }
        string TotalInterstitialAdsProperty { get; }
        string TotalRewardedAdsProperty     { get; }
    }
}