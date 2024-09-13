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
            signalBus.Subscribe<OnIAPPurchaseSuccessSignal>(this.OnIapPurchaseSucceed);
            signalBus.Subscribe<AdRevenueSignal>(this.OnAdRevenueHandler);
            signalBus.Subscribe<AdRevenueRequestSignal>(this.OnAdRevenueRequestHandler);
            signalBus.Subscribe<AdRevenueLoadedSignal>(this.OnAdRevenueLoadedHandler);
            signalBus.Subscribe<AdRevenueClickedSignal>(this.OnAdRevenueClickedHandler);
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

        private void OnAdRevenueHandler(AdRevenueSignal signal)
        {
            this.TrackAdEvent("track_ad_impression", signal.AdsRevenueEvent);    
        }
        
        private void OnAdRevenueRequestHandler(AdRevenueRequestSignal signal)
        {
            this.TrackAdEvent("track_ad_request", signal.AdsRevenueRequestEvent);
        }
        
        private void OnAdRevenueClickedHandler(AdRevenueClickedSignal signal)
        {
            this.TrackAdEvent("track_ad_click", signal.AdsRevenueClickedEvent);
        }
        
        private void OnAdRevenueLoadedHandler(AdRevenueLoadedSignal signal)
        {
            this.TrackAdEvent("track_ad_matched_request", signal.AdsRevenueLoadEvent);
        }
       
        private void OnIapPurchaseSucceed(OnIAPPurchaseSuccessSignal signal)
        {
            this.analyticServices.Track(new Purchase(signal.Product));
        }

        private void TrackAdEvent(string eventName,AdsRevenueEvent adsRevenueEvent)
        {
            this.analyticServices.Track(new CustomEvent
            {
                EventName = eventName,
                EventProperties = new()
                {
                    { "ad_platform", adsRevenueEvent.AdsRevenueSourceId },
                    { "ad_unit_id", adsRevenueEvent.AdUnit },
                    { "ad_source", adsRevenueEvent.AdNetwork },
                    { "ad_source_unit_id", adsRevenueEvent.NetworkPlacement },
                    { "ad_format", adsRevenueEvent.AdFormat },
                    { "currency", adsRevenueEvent.Currency },
                    { "value", adsRevenueEvent.Revenue },
                },
            });
        }
    }
}
#endif
