namespace TheOneStudio.UITemplate.UITemplate.Scripts.ThirdPartyServices.AnalyticEvents.AdOne
{
    using Core.AnalyticServices.Data;

    public class InterstitialAdsSuccess : IEvent
    {
        public string placement;
        public InterstitialAdsSuccess(string placement) { this.placement = placement; }
    }
}