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

        private void OnAdRevenueHandler(AdRevenueSignal signal)
        {
            this.analyticServices.Track(new Rev()); 
            this.analyticServices.Track(new CustomEvent
            {
                EventName = "track_ad_impression",
                EventProperties = new()
                {
                    { "ad_platform", signal.AdsRevenueEvent.AdsRevenueSourceId },
                    { "ad_source", signal.AdsRevenueEvent.AdNetwork },
                    { "ad_unit_id", signal.AdsRevenueEvent.AdUnit },
                    { "ad_format", signal.AdsRevenueEvent.AdFormat },
                    { "placement", signal.AdsRevenueEvent.Placement },
                    { "currency", signal.AdsRevenueEvent.Currency },
                    { "value", signal.AdsRevenueEvent.Revenue },
                },
            });
        }

        private void OnAdRevenueClickedHandler(AdRevenueClickedSignal signal)
        {
            this.analyticServices.Track(new CustomEvent
            {
                EventName = "track_ad_click",
                EventProperties = new()
                {
                    { "ad_platform", signal.AdsRevenueEvent.AdsRevenueSourceId },
                    { "ad_source", signal.AdsRevenueEvent.AdNetwork },
                    { "ad_unit_id", signal.AdsRevenueEvent.AdUnit },
                    { "ad_format", signal.AdsRevenueEvent.AdFormat },
                    { "placement", signal.AdsRevenueEvent.Placement },
                    { "currency", signal.AdsRevenueEvent.Currency },
                    { "value", signal.AdsRevenueEvent.Revenue },
                },
            });
        }

        private void OnAdRevenueLoadedHandler(AdRevenueLoadedSignal signal)
        {
            this.analyticServices.Track(new CustomEvent
            {
                EventName = "track_ad_matched_request",
                EventProperties = new()
                {
                    { "ad_platform", signal.AdsRevenueEvent.AdsRevenueSourceId },
                    { "ad_source", signal.AdsRevenueEvent.AdNetwork },
                    { "ad_unit_id", signal.AdsRevenueEvent.AdUnit },
                    { "ad_format", signal.AdsRevenueEvent.AdFormat },
                    { "placement", signal.AdsRevenueEvent.Placement },
                    { "currency", signal.AdsRevenueEvent.Currency },
                    { "value", signal.AdsRevenueEvent.Revenue },
                },
            });
        }
    }
}
#endif