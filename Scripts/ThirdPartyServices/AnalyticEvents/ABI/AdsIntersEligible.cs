namespace TheOneStudio.UITemplate.UITemplate.Scripts.ThirdPartyServices.AnalyticEvents.ABI
{
    using Core.AnalyticServices.Data;

    public class AdsIntersEligible : IEvent
    {
        public readonly string place;
        public AdsIntersEligible(string place) { this.place = place; }
    }
}