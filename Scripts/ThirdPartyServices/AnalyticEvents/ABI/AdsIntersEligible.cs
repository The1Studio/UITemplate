namespace TheOneStudio.UITemplate.UITemplate.Scripts.ThirdPartyServices.AnalyticEvents.ABI
{
    using Core.AnalyticServices.Data;

    public class AdsIntersEligible : IEvent
    {
        public readonly string placement;
        public AdsIntersEligible(string placement) { this.placement = placement; }
    }
}