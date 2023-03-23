namespace TheOneStudio.UITemplate.UITemplate.Scripts.ThirdPartyServices.AnalyticEvents.AdOne
{
    using Core.AnalyticServices.Data;

    public class AdInterClick:IEvent
    {
        public string Placement;

        public AdInterClick(string placement)
        {
            this.Placement = placement;
        }
    }
}