namespace TheOneStudio.UITemplate.UITemplate.ThirdPartyServices.AnalyticEvents.Wido
{
    using Core.AnalyticServices.Data;

    public class InterstitialAdsSuccess : IEvent
    {
        public string placement;
        public InterstitialAdsSuccess(string placement) { this.placement = placement; }
    }
}