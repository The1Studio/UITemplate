namespace TheOneStudio.UITemplate.UITemplate.ThirdPartyServices.AnalyticEvents.Apero
{
    using Core.AnalyticServices.CommonEvents;
    using Core.AnalyticServices.Data;

    public class PurchaseEvent : IEvent
    {
        public IapTransactionDidSucceed TransactionData;
    }
}