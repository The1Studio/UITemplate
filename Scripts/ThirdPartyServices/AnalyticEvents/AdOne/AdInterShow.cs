namespace TheOneStudio.UITemplate.UITemplate.Scripts.ThirdPartyServices.AnalyticEvents.AdOne
{
    using Core.AnalyticServices.Data;

    public class AdInterShow : IEvent
    {
        public string Placement;

        public AdInterShow(string placement) { this.Placement = placement; }
    }
}