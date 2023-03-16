namespace TheOneStudio.UITemplate.UITemplate.ThirdPartyServices.AnalyticEvents.ABI
{
    using Core.AnalyticServices.Data;

    public class AdInterLoad : IEvent
    {
        public string Placement;

        public AdInterLoad(string placement) { this.Placement = placement; }
    }
}