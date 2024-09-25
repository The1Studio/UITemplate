namespace TheOneStudio.UITemplate.UITemplate.ThirdPartyServices.AnalyticEvents.Apero
{
    using Core.AnalyticServices.CommonEvents;
    using Core.AnalyticServices.Data;
    using ServiceImplementation.IAPServices;

    public class Purchase : IEvent
    {
        public ProductData ProductData;
        public Purchase(ProductData productData) { this.ProductData = productData; }
    }
}