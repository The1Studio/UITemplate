namespace TheOneStudio.UITemplate.UITemplate.ThirdPartyServices.AnalyticEvents
{
    using System.Collections.Generic;
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

        IEvent RewardedVideoShowFail(string   place, string msg);
        IEvent RewardedLoadFail(string        place, string msg);
        IEvent RewardedVideoShowCompleted(int level, string place, bool isRewarded);

        IEvent RewardedVideoClick(string place);

        IEvent AppOpenCalled(string   place);
        IEvent AppOpenEligible(string place);
        IEvent AppOpenLoadFailed();
        IEvent AppOpenLoaded();
        IEvent AppOpenFullScreenContentClosed(string place);
        IEvent AppOpenFullScreenContentFailed(string place);
        IEvent AppOpenFullScreenContentOpened(string place);
        IEvent AppOpenClicked(string                 place);

        IEvent LevelLose(int    level, int timeSpent, int loseCount);
        IEvent LevelStart(int   level, int gold,      int totalLevelsPlayed, long timestamp, int gameModeId, int totalLevelsTypePlayed);
        IEvent LevelWin(int     level, int timeSpent, int winCount);
        IEvent FirstWin(int     level, int timeSpent);
        IEvent LevelSkipped(int level, int timeSpent);


        IEvent EarnResource(string resourceId, long value, string source, Dictionary<string, object> spentResources, long timestamp);
        IEvent SpendResource(string resourceId, long value, string location, long timestamp);

        IEvent TutorialCompletion(bool success, string tutorialId, int stepId = 0, string stepName = "");

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