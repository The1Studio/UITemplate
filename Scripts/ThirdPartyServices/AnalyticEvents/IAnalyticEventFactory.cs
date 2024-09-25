namespace TheOneStudio.UITemplate.UITemplate.ThirdPartyServices.AnalyticEvents
{
    using Core.AnalyticServices.Data;

    public interface IAnalyticEventFactory
    {
        IEvent BannerAdShow();
        IEvent BannerAdLoadFail(string msg);
        IEvent BannerAdLoad();
        IEvent InterstitialEligible(string place);

        IEvent InterstitialShow(int level, string place);

        IEvent InterstitialShowCompleted(int level, string place);

        IEvent InterstitialShowFail(string place, string msg);

        IEvent InterstitialClick(string place);

        IEvent InterstitialDownloaded(string place, long loadingMilis);

        IEvent InterstitialDownloadFailed(string place, string message, long loadingMilis);

        IEvent InterstitialCalled(string place);

        IEvent RewardedInterstitialAdDisplayed(int level, string place);

        IEvent RewardedVideoEligible(string place);

        IEvent RewardedVideoOffer(string place);

        IEvent RewardedVideoDownloaded(string place, long loadingMilis);

        IEvent RewardedVideoDownloadFailed(string place, long loadingMilis);

        IEvent RewardedVideoCalled(string place);

        IEvent RewardedVideoShow(int level, string place);

        IEvent RewardedVideoShowFail(string place, string msg);
        IEvent RewardedLoadFail(string place, string msg);
        IEvent RewardedVideoShowCompleted(int level, string place, bool isRewarded);

        IEvent RewardedVideoClick(string place);

        IEvent AppOpenCalled();
        IEvent AppOpenEligible();
        IEvent AppOpenLoadFailed();
        IEvent AppOpenLoaded();
        IEvent AppOpenFullScreenContentClosed();
        IEvent AppOpenFullScreenContentFailed();
        IEvent AppOpenFullScreenContentOpened();
        IEvent AppOpenClicked();

        IEvent LevelLose(int level, int timeSpent, int loseCount);

        IEvent LevelStart(int level, int gold);

        IEvent LevelWin(int level, int timeSpent, int winCount);

        IEvent FirstWin(int level, int timeSpent);

        IEvent LevelSkipped(int level, int timeSpent);

        IEvent EarnVirtualCurrency(string virtualCurrencyName, long value, string source);

        IEvent SpendVirtualCurrency(string virtualCurrencyName, long value, string itemName);

        IEvent TutorialCompletion(bool success, string tutorialId);

        void ForceUpdateAllProperties();

        string                            LevelMaxProperty                           { get; }
        string                            LastLevelProperty                          { get; }
        string                            LastAdsPlacementProperty                   { get; }
        string                            TotalInterstitialAdsProperty               { get; }
        string                            TotalRewardedAdsProperty                   { get; }
        string                            TotalVirtualCurrencySpentProperty          { get; }
        string                            TotalVirtualCurrencyEarnedProperty         { get; }
        string                            DaysPlayedProperty                         { get; }
        string                            RetentionDayProperty                       { get; }
        AnalyticsEventCustomizationConfig AppsFlyerAnalyticsEventCustomizationConfig { get; set; }
        AnalyticsEventCustomizationConfig FireBaseAnalyticsEventCustomizationConfig  { get; set; }
#if BYTEBREW
        AnalyticsEventCustomizationConfig ByteBrewAnalyticsEventCustomizationConfig { get; set; }
#endif
    }
}