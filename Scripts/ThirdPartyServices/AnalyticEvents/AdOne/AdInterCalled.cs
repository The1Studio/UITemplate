namespace TheOneStudio.UITemplate.UITemplate.Scripts.ThirdPartyServices.AnalyticEvents.AdOne
{
    using Core.AnalyticServices.Data;

    public class AdInterCalled : IEvent
    {
        public string Placement;

        public AdInterCalled(string placement) { this.Placement = placement; }
    }
}