#if APERO
namespace TheOneStudio.UITemplate.UITemplate.ThirdPartyServices.AnalyticEvents.Apero
{
    using Core.AnalyticServices;
    using Core.AnalyticServices.CommonEvents;
    using Core.AnalyticServices.Data;
    using Core.AnalyticServices.Signal;
    using Zenject;

    public class AperoAnalyticEventFactory : BaseAnalyticEventFactory
    {
        public AperoAnalyticEventFactory(SignalBus signalBus, IAnalyticServices analyticServices) : base(signalBus, analyticServices)
        {
            signalBus.Subscribe<IapTransactionDidSucceed>(this.OnTransactionSucceedHandler);
            signalBus.Subscribe<AdRevenueSignal>(this.OnAdRevenueHandler);
        }

        public override AnalyticsEventCustomizationConfig ByteBrewAnalyticsEventCustomizationConfig { get; set; } = new()
        {
            IgnoreEvents = new(),
            CustomEventKeys = new()
            {
                { nameof(RevEvent), "Rev" },
                { nameof(PurchaseEvent), "Purchase" },
            },
        };

        private void OnTransactionSucceedHandler(IapTransactionDidSucceed signal)
        {
            this.analyticServices.Track(new PurchaseEvent
            {
                TransactionData = signal,
            });
        }

        public override AnalyticsEventCustomizationConfig FireBaseAnalyticsEventCustomizationConfig { get; set; }

        private void OnAdRevenueHandler(AdRevenueSignal signal) { this.analyticServices.Track(new RevEvent()); }
    }
}
#endif
