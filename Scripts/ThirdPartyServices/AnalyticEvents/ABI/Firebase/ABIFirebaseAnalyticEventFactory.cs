namespace TheOneStudio.UITemplate.UITemplate.ThirdPartyServices.AnalyticEvents.ABI.Firebase
{
    using System.Collections.Generic;
    using Core.AnalyticServices.CommonEvents;
    using Core.AnalyticServices.Data;

    public class ABIFirebaseAnalyticEventFactory : IAnalyticEventFactory
    {
        public IEvent InterstitialShow(int level, string place) => new AdInterLoad(place);

        public IEvent InterstitialShowCompleted(int level, string place) => new AdInterShow(place);

        public IEvent InterstitialShowFail(string place, string msg) => new AdInterFail(msg);

        public IEvent InterstitialClick(string place) => new AdInterClick(place);

        public IEvent RewardedVideoOffer(string place) => new AdsRewardOffer(place);

        public IEvent RewardedVideoShow(int level, string place) => new AdsRewardOffer(place);

        public IEvent RewardedVideoShowCompleted(int level, string place, bool isRewarded) => new AdsRewardShow(place);

        public IEvent RewardedVideoClick(string place) => new AdsRewardClick(place);

        public IEvent RewardedVideoShowFail(string place, string msg) => new AdsRewardFail(place, msg);

        public IEvent LevelLose(int level, int timeSpent, int loseCount) => new LevelFail(level, loseCount);

        public IEvent LevelStart(int level, int gold) => new LevelStart(level, gold);

        public IEvent LevelWin(int level, int timeSpent, int winCount) => new LevelComplete(level, timeSpent);

        public IEvent FirstWin(int level, int timeSpent) => new CustomEvent
        {
            EventName       = $"checkpoints_{level}",
            EventProperties = new Dictionary<string, object>()
        };

        public IEvent LevelSkipped(int level, int timeSpent) => new LevelSkipped(level, timeSpent);

        public IEvent EarnVirtualCurrency(string virtualCurrencyName, long value, string source) => new EarnVirtualCurrency(virtualCurrencyName, value, source);

        public IEvent SpendVirtualCurrency(string virtualCurrencyName, long value, string itemName) => new SpendVirtualCurrency(virtualCurrencyName, value, itemName);

        public void ForceUpdateAllProperties() { }

        public string LevelMaxProperty             => "level_max";
        public string LastLevelProperty            => "last_level";
        public string LastAdsPlacementProperty     => "last_placement";
        public string TotalInterstitialAdsProperty => "total_interstitial_ads";
        public string TotalRewardedAdsProperty     => "total_rewarded_ads";
    }
}