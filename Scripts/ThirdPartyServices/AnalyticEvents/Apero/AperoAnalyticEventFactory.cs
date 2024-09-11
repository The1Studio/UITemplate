#if APERO
namespace TheOneStudio.UITemplate.UITemplate.ThirdPartyServices.AnalyticEvents.Apero
{
    using Core.AnalyticServices;
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

        private void OnAdRevenueHandler(AdRevenueSignal signal) { this.analyticServices.Track(new Rev()); }
    }
}
#endif