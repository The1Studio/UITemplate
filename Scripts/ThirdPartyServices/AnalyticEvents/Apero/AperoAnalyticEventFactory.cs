#if APERO
namespace TheOneStudio.UITemplate.UITemplate.ThirdPartyServices.AnalyticEvents.Apero
{
    using Core.AdsServices.Signals;
    using Core.AnalyticServices;
    using Core.AnalyticServices.CommonEvents;
    using Core.AnalyticServices.Data;
    using Core.AnalyticServices.Signal;
    using GameFoundation.Signals;
    using ServiceImplementation.IAPServices.Signals;
    using AdInfo = Core.AdsServices.AdInfo;
    using UnityEngine.Scripting;

    public class AperoAnalyticEventFactory : BaseAnalyticEventFactory
    {
        [Preserve]
        public AperoAnalyticEventFactory(SignalBus signalBus, IAnalyticServices analyticServices) : base(signalBus, analyticServices)
        {
            signalBus.Subscribe<OnIAPPurchaseSuccessSignal>(this.OnIAPPurchaseSuccessHandler);
            signalBus.Subscribe<AdRevenueSignal>(this.OnAdRevenueHandler);
            signalBus.Subscribe<AdRequestSignal>(signal => this.TrackAdEvent("track_ad_request", signal.AdInfo));

            signalBus.Subscribe<BannerAdLoadedSignal>(signal => this.TrackAdEvent("track_ad_matched_request", signal.AdInfo));
            signalBus.Subscribe<BannerAdClickedSignal>(signal => this.TrackAdEvent("track_ad_click", signal.AdInfo));

            signalBus.Subscribe<InterstitialAdLoadedSignal>(signal => this.TrackAdEvent("track_ad_matched_request", signal.AdInfo));
            signalBus.Subscribe<InterstitialAdClickedSignal>(signal => this.TrackAdEvent("track_ad_click", signal.AdInfo));

            signalBus.Subscribe<RewardedAdLoadedSignal>(signal => this.TrackAdEvent("track_ad_matched_request", signal.AdInfo));
            signalBus.Subscribe<RewardedAdClickedSignal>(signal => this.TrackAdEvent("track_ad_click", signal.AdInfo));

            signalBus.Subscribe<AppOpenLoadedSignal>(signal => this.TrackAdEvent("track_ad_matched_request", signal.AdInfo));
            signalBus.Subscribe<AppOpenFullScreenContentClosedSignal>(signal => this.TrackAdEvent("track_ad_click", signal.AdInfo));
        }

        public override AnalyticsEventCustomizationConfig ByteBrewAnalyticsEventCustomizationConfig { get; set; } = new()
        {
            IgnoreEvents = new(),
            CustomEventKeys = new()
            {
                { nameof(Rev), "Rev" },
                { nameof(Purchase), "Purchase" },
            },
        };

        public override AnalyticsEventCustomizationConfig FireBaseAnalyticsEventCustomizationConfig { get; set; } = new()
        {
            IgnoreEvents = new(),
            CustomEventKeys = new()
            {
                { nameof(Rev), "Rev" },
                { nameof(Purchase), "Purchase" },
            },
        };

        private void OnIAPPurchaseSuccessHandler(OnIAPPurchaseSuccessSignal signal) { this.analyticServices.Track(new Purchase(signal.Product)); }

        private void OnAdRevenueHandler(AdRevenueSignal signal)
        {
            this.TrackAdEvent("track_ad_impression",
                new(signal.AdsRevenueEvent.AdsRevenueSourceId,
                    signal.AdsRevenueEvent.AdUnit,
                    signal.AdsRevenueEvent.AdFormat,
                    signal.AdsRevenueEvent.AdNetwork,
                    signal.AdsRevenueEvent.NetworkPlacement,
                    signal.AdsRevenueEvent.Revenue,
                    signal.AdsRevenueEvent.Currency));
        }

        private void TrackAdEvent(string eventName, AdInfo adInfo)
        {
            this.analyticServices.Track(new CustomEvent
            {
                EventName = eventName,
                EventProperties = new()
                {
                    { "ad_platform", adInfo.AdPlatform },
                    { "ad_unit_id", adInfo.AdUnitId },
                    { "ad_source", adInfo.AdSource },
                    { "ad_source_unit_id", adInfo.AdSourceUnitId },
                    { "ad_format", adInfo.AdFormat },
                    { "currency", adInfo.Currency },
                    { "value", adInfo.Value },
                },
            });
        }
    }
}
#endif