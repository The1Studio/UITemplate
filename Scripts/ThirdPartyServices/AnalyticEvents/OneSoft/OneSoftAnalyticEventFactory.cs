namespace TheOneStudio.UITemplate.UITemplate.ThirdPartyServices.AnalyticEvents.OneSoft
{
    using Core.AnalyticServices.CommonEvents;
    using Core.AnalyticServices.Data;

    public class OneSoftAnalyticEventFactory : IAnalyticEventFactory
    {
        public IEvent InterstitialShow(int level, string place) => new InterstitialShow(level, place);

        public IEvent InterstitialShowCompleted(int level, string place) => new InterstitialShowCompleted(place);

        public IEvent InterstitialShowFail(string place, string msg) => new CustomEvent();

        public IEvent InterstitialClick(string place) => new CustomEvent();

        public IEvent RewardedVideoShow(int level, string place) => new RewardedVideoShow(level, place);

        public IEvent RewardedVideoShowCompleted(int level, string place, bool isRewarded, string msg) => new RewardedAdsShowCompleted(place, isRewarded ? "success" : "skip");

        public IEvent RewardedVideoClick(string place) { return new CustomEvent(); }

        public IEvent RewardedVideoShowFail(string place, string msg) { return new CustomEvent(); }

        public IEvent LevelLose(int level, int timeSpent, int loseCount) => new LevelLose(level, timeSpent);

        public IEvent LevelStart(int level, int gold) => new LevelStart(level);

        public IEvent LevelWin(int level, int timeSpent, int winCount) => new LevelWin(level, timeSpent);

        public IEvent FirstWin(int level, int timeSpent) => new CustomEvent();

        public IEvent LevelSkipped(int level, int timeSpent) => new LevelSkipped(level, timeSpent);

        public IEvent EarnVirtualCurrency(string virtualCurrencyName, long value, string source) { return new CustomEvent(); }

        public IEvent SpendVirtualCurrency(string virtualCurrencyName, long value, string itemName) { return new CustomEvent(); }

        public IEvent EarnVirtualCurrency(string type, int amount) { return new CustomEvent(); }

        public void ForceUpdateAllProperties() { }

        public string LevelMaxProperty             => "level_max";
        public string LastLevelProperty            => "last_level";
        public string LastAdsPlacementProperty     => "last_placement";
        public string TotalInterstitialAdsProperty => "total_interstitial_ads";
        public string TotalRewardedAdsProperty     => "total_rewarded_ads";
    }
}