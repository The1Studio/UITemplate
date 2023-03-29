namespace TheOneStudio.UITemplate.UITemplate.ThirdPartyServices.AnalyticEvents.Rocket
{
    using Core.AnalyticServices.Data;

    public class InterstitialShowCompleted : IEvent
    {
        public string placement;
        public InterstitialShowCompleted(string placement) { this.placement = placement; }
    }
}