#if APERO
namespace TheOneStudio.UITemplate.UITemplate.ThirdPartyServices.AnalyticEvents.Apero
{
    using Core.AnalyticServices;
    using Core.AnalyticServices.CommonEvents;
    using Core.AnalyticServices.Data;
    using Core.AnalyticServices.Signal;
    using ServiceImplementation.IAPServices.Signals;
    using Zenject;

    public class AperoAnalyticEventFactory : BaseAnalyticEventFactory
    {
        public AperoAnalyticEventFactory(SignalBus signalBus, IAnalyticServices analyticServices) : base(signalBus, analyticServices)
        {
            signalBus.Subscribe<OnIAPPurchaseSuccessSignal>(this.OnIAPPurchaseSuccessHandler);
            signalBus.Subscribe<AdRevenueSignal>(this.OnAdRevenueHandler);
            signalBus.Subscribe<AdRevenueClickedSignal>(this.OnAdRevenueClickedHandler);
            signalBus.Subscribe<AdRevenueLoadedSignal>(this.OnAdRevenueLoadedHandler);
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

        private void OnAdRevenueHandler(AdRevenueSignal signal) { this.TrackAdEvent("track_ad_impression", signal.NetworkPlacement, signal.AdsRevenueEvent); }

        private void OnAdRevenueClickedHandler(AdRevenueClickedSignal signal) { this.TrackAdEvent("track_ad_click", signal.NetworkPlacement, signal.AdsRevenueEvent); }

        private void OnAdRevenueLoadedHandler(AdRevenueLoadedSignal signal) { this.TrackAdEvent("track_ad_matched_request", signal.NetworkPlacement, signal.AdsRevenueEvent); }

        private void TrackAdEvent(string eventName, string placement, AdsRevenueEvent adsRevenueEvent)
        {
            this.analyticServices.Track(new CustomEvent
            {
                EventName = eventName,
                EventProperties = new()
                {
                    { "ad_platform", adsRevenueEvent.AdsRevenueSourceId },
                    { "ad_unit_id", adsRevenueEvent.AdUnit },
                    { "ad_source", adsRevenueEvent.AdNetwork },
                    { "ad_source_unit_id", placement },
                    { "ad_format", adsRevenueEvent.AdFormat },
                    { "placement", adsRevenueEvent.Placement },
                    { "currency", adsRevenueEvent.Currency },
                    { "value", adsRevenueEvent.Revenue },
                },
            });
        }

        private void TrackAdRequestEvent()
        {
            
        }
    }
}
#endif