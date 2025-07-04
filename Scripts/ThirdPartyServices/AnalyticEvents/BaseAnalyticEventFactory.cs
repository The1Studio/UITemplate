﻿namespace TheOneStudio.UITemplate.UITemplate.ThirdPartyServices.AnalyticEvents
{
    using Core.AnalyticServices;
    using Core.AnalyticServices.CommonEvents;
    using Core.AnalyticServices.Data;
    using Core.AnalyticServices.Signal;
    using GameFoundation.Signals;
    using TheOneStudio.UITemplate.UITemplate.Models.Controllers;
    using TheOneStudio.UITemplate.UITemplate.ThirdPartyServices.AnalyticEvents.CommonEvents;

    public abstract class BaseAnalyticEventFactory : IAnalyticEventFactory
    {
        //Interstitial ads
        public virtual IEvent BannerAdShow()
        {
            return new BannerShown();
        }

        public virtual IEvent BannerAdLoadFail(string msg)
        {
            return new BannerLoadFail(msg);
        }

        public virtual IEvent BannerAdLoad()
        {
            return new BannerLoad();
        }

        public virtual IEvent InterstitialEligible(string place)
        {
            return new InterstitialAdEligible(place);
        }

        public virtual IEvent InterstitialShow(int level, string place)
        {
            return new InterstitialAdDisplayed(level, place);
        }

        public virtual IEvent InterstitialShowCompleted(int level, string place)
        {
            return new InterstitialAdClosed(place);
        }

        public virtual IEvent InterstitialShowFail(string place, string msg)
        {
            return new InterstitialAdDisplayedFailed(place);
        }

        public virtual IEvent InterstitialClick(string place)
        {
            return new InterstitialAdClicked(place);
        }

        public virtual IEvent InterstitialDownloaded(string place, long loadingMilis)
        {
            return new InterstitialAdDownloaded(place, loadingMilis);
        }

        public virtual IEvent InterstitialDownloadFailed(string place, string message, long loadingMilis)
        {
            return new InterstitialAdLoadFailed(place, message, loadingMilis);
        }

        public virtual IEvent InterstitialCalled(string place)
        {
            return new InterstitialAdCalled(place);
        }

        //Rewarded Interstitital
        public virtual IEvent RewardedInterstitialAdDisplayed(int level, string place)
        {
            return new CustomEvent();
        }

        //RewardVideo Ads
        public virtual IEvent RewardedVideoEligible(string place)
        {
            return new RewardedAdEligible(place);
        }

        public virtual IEvent RewardedVideoOffer(string place)
        {
            return new RewardedAdOffer(place);
        }

        public virtual IEvent RewardedVideoDownloaded(string place, long loadingMilis)
        {
            return new RewardedAdLoaded(place, loadingMilis);
        }

        public virtual IEvent RewardedVideoDownloadFailed(string place, long loadingMilis)
        {
            return new RewardedAdLoadFailed(place, loadingMilis, "");
        }

        public virtual IEvent RewardedVideoCalled(string place)
        {
            return new RewardedAdCalled(place);
        }

        public virtual IEvent RewardedVideoShow(int level, string place)
        {
            return new RewardedAdDisplayed(place, level);
        }

        public IEvent RewardedLoadFail(string place, string msg)
        {
            return new RewardedAdLoadFailed(place, 0, msg);
        }

        public virtual IEvent RewardedVideoShowCompleted(int level, string place, bool isRewarded)
        {
            return isRewarded ? new RewardedAdCompleted(place) : new RewardedSkipped(place);
        }

        public virtual IEvent RewardedVideoClick(string place)
        {
            return new RewardedAdLoadClicked(place);
        }

        public virtual IEvent RewardedVideoShowFail(string place, string msg)
        {
            return new RewardedAdShowFail(place);
        }

        //App open
        public virtual IEvent AppOpenCalled(string place)
        {
            return new AppOpenCalled(place);
        }

        public virtual IEvent AppOpenEligible(string place)
        {
            return new AppOpenEligible(place);
        }

        public virtual IEvent AppOpenLoadFailed()
        {
            return new AppOpenLoadFailed();
        }

        public virtual IEvent AppOpenLoaded()
        {
            return new AppOpenLoaded();
        }

        public virtual IEvent AppOpenFullScreenContentClosed(string place)
        {
            return new AppOpenFullScreenContentClosed(place);
        }

        public virtual IEvent AppOpenFullScreenContentFailed(string place)
        {
            return new AppOpenFullScreenContentFailed(place);
        }

        public virtual IEvent AppOpenFullScreenContentOpened(string place)
        {
            return new AppOpenFullScreenContentOpened(place);
        }

        public virtual IEvent AppOpenClicked(string place)
        {
            return new AppOpenClicked(place);
        }

        //Level
        public virtual IEvent LevelStart(int level, int gold)
        {
            return new LevelStart(level, gold);
        }

        public virtual IEvent LevelWin(int level, int timeSpent, int winCount)
        {
            return new LevelWin(level, timeSpent);
        }

        public virtual IEvent LevelLose(int level, int timeSpent, int loseCount)
        {
            return new LevelLose(level, timeSpent);
        }

        public virtual IEvent FirstWin(int level, int timeSpent)
        {
            return new FirstWin(level, timeSpent);
        }

        public virtual IEvent LevelSkipped(int level, int timeSpent)
        {
            return new LevelSkipped(level, timeSpent);
        }
        public IEvent EarnVirtualCurrency(string virtualCurrencyName, long value, string placement, int level)
        {
            return new EarnVirtualCurrency(virtualCurrencyName, value, placement, level);

        }
        public IEvent SpendVirtualCurrency(string virtualCurrencyName, long value, string placement, int level)
        {
            return new SpendVirtualCurrency(virtualCurrencyName, value, placement, level);
        }

        public virtual IEvent TutorialCompletion(bool success, string tutorialId)
        {
            return new TutorialCompletion(success, tutorialId);
        }

        public virtual void ForceUpdateAllProperties()
        {
        }

        public virtual string LevelMaxProperty                   => "level_max";
        public virtual string LastLevelProperty                  => "last_level";
        public virtual string LastAdsPlacementProperty           => "last_placement";
        public virtual string TotalInterstitialAdsProperty       => "total_interstitial_ads";
        public virtual string TotalRewardedAdsProperty           => "total_rewarded_ads";
        public virtual string TotalVirtualCurrencySpentProperty  => "total_spent";
        public virtual string TotalVirtualCurrencyEarnedProperty => "total_earned";
        public virtual string DaysPlayedProperty                 => "days_played";
        public virtual string RetentionDayProperty               => "retention";

        public virtual AnalyticsEventCustomizationConfig AppsFlyerAnalyticsEventCustomizationConfig { get; set; } = new();
        public virtual AnalyticsEventCustomizationConfig FireBaseAnalyticsEventCustomizationConfig  { get; set; } = new();
        public virtual AnalyticsEventCustomizationConfig ByteBrewAnalyticsEventCustomizationConfig  { get; set; } = new();

        protected virtual string EvenName { get; } = "ad_impression";

        protected readonly IAnalyticServices             analyticServices;
        private readonly   UITemplateLevelDataController levelDataController;

        protected BaseAnalyticEventFactory(SignalBus signalBus, IAnalyticServices analyticServices, UITemplateLevelDataController levelDataController)
        {
            this.analyticServices    = analyticServices;
            this.levelDataController = levelDataController;
            signalBus.Subscribe<AdRevenueSignal>(this.OnAdsRevenue);
        }

        protected virtual void OnAdsRevenue(AdRevenueSignal obj)
        {
            this.analyticServices.Track(new CustomEvent()
            {
                EventName = this.EvenName,
                EventProperties = new()
                {
                    { "ad_platform", obj.AdsRevenueEvent.AdsRevenueSourceId },
                    { "ad_source", obj.AdsRevenueEvent.AdNetwork },
                    { "ad_unit_name", obj.AdsRevenueEvent.AdUnit },
                    { "ad_format", obj.AdsRevenueEvent.AdFormat },
                    { "placement", obj.AdsRevenueEvent.Placement },
                    { "currency", obj.AdsRevenueEvent.Currency },
                    { "value", obj.AdsRevenueEvent.Revenue },
                    { "level_id", this.levelDataController.CurrentLevel }
                },
            });
        }
    }
}