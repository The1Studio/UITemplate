namespace TheOneStudio.UITemplate.UITemplate.Scripts.ThirdPartyServices.AnalyticEvents.AdOne
{
    using Core.AnalyticServices.Data;

    public class AdInterLoad:IEvent
    {
        public string Placement;

        public AdInterLoad(string placement) { this.Placement = placement; }
    }
}