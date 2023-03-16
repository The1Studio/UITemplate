namespace TheOneStudio.UITemplate.UITemplate.ThirdPartyServices.AnalyticEvents.ABI.Appsflyer
{
    using Core.AnalyticServices.Data;

    public class ABIAppsflyerAnalyticEventFactory : IAnalyticEventFactory
    {
        public IEvent InterstitialShow(int level, string place) { throw new System.NotImplementedException(); }

        public IEvent InterstitialShowCompleted(int level, string place) { throw new System.NotImplementedException(); }

        public IEvent InterstitialShowFail(string place, string msg) { throw new System.NotImplementedException(); }

        public IEvent InterstitialClick(string place) { throw new System.NotImplementedException(); }

        public IEvent RewardedVideoOffer(string place) { throw new System.NotImplementedException(); }

        public IEvent RewardedVideoShow(int level, string place) { throw new System.NotImplementedException(); }

        public IEvent RewardedVideoShowCompleted(int level, string place, bool isRewarded) { throw new System.NotImplementedException(); }

        public IEvent RewardedVideoClick(string place) { throw new System.NotImplementedException(); }

        public IEvent RewardedVideoShowFail(string place, string msg) { throw new System.NotImplementedException(); }

        public IEvent LevelLose(int level, int timeSpent, int loseCount) { throw new System.NotImplementedException(); }

        public IEvent LevelStart(int level, int gold) { throw new System.NotImplementedException(); }

        public IEvent LevelWin(int level, int timeSpent, int winCount) { throw new System.NotImplementedException(); }

        public IEvent LevelStart(int level) { throw new System.NotImplementedException(); }

        public IEvent LevelWin(int level, int timeSpent) { throw new System.NotImplementedException(); }

        public IEvent FirstWin(int level, int timeSpent) { throw new System.NotImplementedException(); }

        public IEvent LevelSkipped(int level, int timeSpent) { throw new System.NotImplementedException(); }

        public IEvent EarnVirtualCurrency(string virtualCurrencyName, long value, string source) { throw new System.NotImplementedException(); }

        public IEvent SpendVirtualCurrency(string virtualCurrencyName, long value, string itemName) { throw new System.NotImplementedException(); }

        public void ForceUpdateAllProperties() { throw new System.NotImplementedException(); }

        public string LevelMaxProperty             { get; }
        public string LastLevelProperty            { get; }
        public string LastAdsPlacementProperty     { get; }
        public string TotalInterstitialAdsProperty { get; }
        public string TotalRewardedAdsProperty     { get; }
    }
}