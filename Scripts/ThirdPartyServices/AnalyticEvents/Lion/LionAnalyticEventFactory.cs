#if LION
namespace TheOneStudio.UITemplate.UITemplate.ThirdPartyServices.AnalyticEvents.Lion
{
    using Core.AnalyticServices;
    using Core.AnalyticServices.Data;
    using GameFoundation.Signals;
    using LionStudios.Suite.Analytics;
    using LionStudios.Suite.Analytics.Events.EventArgs;

    public class LionAnalyticEventFactory : BaseAnalyticEventFactory
    {
        public LionAnalyticEventFactory(SignalBus signalBus, IAnalyticServices analyticServices) : base(signalBus, analyticServices)
        {
        }

        public override IEvent AppOpenCalled(string place)
        {
            LionAnalytics.AppOpenClicked(new());

            return base.AppOpenCalled(place);
        }

        public override IEvent AppOpenLoadFailed()
        {
            LionAnalytics.AppOpenShowFail(new());

            return base.AppOpenLoadFailed();
        }

        public override IEvent AppOpenLoaded()
        {
            LionAnalytics.AppOpenLoad(new());

            return base.AppOpenLoaded();
        }

        public override IEvent BannerAdShow()
        {
            LionAnalytics.BannerShow(new BannerEventArgs());

            return base.BannerAdShow();
        }

        public override IEvent BannerAdLoadFail(string msg)
        {
            LionAnalytics.BannerLoadFail(new BannerFailEventArgs());

            return base.BannerAdLoadFail(msg);
        }

        public override IEvent BannerAdLoad()
        {
            LionAnalytics.BannerShowRequested(new BannerEventArgs());

            return base.BannerAdLoad();
        }

        public override IEvent AppOpenFullScreenContentOpened(string place)
        {
            LionAnalytics.AppOpenShow(new() { Placement = place });

            return base.AppOpenFullScreenContentOpened(place);
        }

        public override IEvent AppOpenClicked(string place)
        {
            LionAnalytics.AppOpenClicked(new() { Placement = place });

            return base.AppOpenClicked(place);
        }

        public override IEvent InterstitialEligible(string place)
        {
            LionAnalytics.InterstitialStart(place);

            return base.InterstitialEligible(place);
        }

        public override IEvent InterstitialShow(int level, string place)
        {
            LionAnalytics.InterstitialShow(placement: place, level: level);

            return base.InterstitialShow(level, place);
        }

        public override IEvent InterstitialShowFail(string place, string msg)
        {
            LionAnalytics.InterstitialShowFail(place);

            return base.InterstitialShowFail(place, msg);
        }

        public override IEvent InterstitialClick(string place)
        {
            LionAnalytics.InterstitialClick(placement: place);

            return base.InterstitialClick(place);
        }

        public override IEvent InterstitialDownloaded(string place, long loadingMilis)
        {
            LionAnalytics.InterstitialLoad(placement: place);

            return base.InterstitialDownloaded(place, loadingMilis);
        }

        public override IEvent InterstitialDownloadFailed(string place, string message, long loadingMilis)
        {
            LionAnalytics.InterstitialLoadFail(place);

            return base.InterstitialDownloadFailed(place, message, loadingMilis);
        }

        public override IEvent InterstitialCalled(string place)
        {
            LionAnalytics.InterstitialShow(place);

            return base.InterstitialCalled(place);
        }

        public override IEvent RewardedVideoDownloaded(string place, long loadingMilis)
        {
            LionAnalytics.RewardVideoLoad(placement: place);

            return base.RewardedVideoDownloaded(place, loadingMilis);
        }

        public override IEvent RewardedVideoDownloadFailed(string place, long loadingMilis)
        {
            LionAnalytics.RewardVideoLoadFail(placement: place);

            return base.RewardedVideoDownloadFailed(place, loadingMilis);
        }

        public override IEvent RewardedVideoCalled(string place)
        {
            LionAnalytics.RewardVideoShowRequested(place);

            return base.RewardedVideoCalled(place);
        }

        public override IEvent RewardedVideoShowCompleted(int level, string place, bool isRewarded)
        {
            LionAnalytics.RewardVideoCollect(placement: place, level: level);

            return base.RewardedVideoShowCompleted(level, place, isRewarded);
        }

        public override IEvent RewardedVideoClick(string place)
        {
            LionAnalytics.RewardVideoClick(place);

            return base.RewardedVideoClick(place);
        }

        public override IEvent RewardedVideoShowFail(string place, string msg)
        {
            LionAnalytics.RewardVideoShowFail(placement: place, additionalData: new() { { "msg", msg } });

            return base.RewardedVideoShowFail(place, msg);
        }
    }
}
#endif