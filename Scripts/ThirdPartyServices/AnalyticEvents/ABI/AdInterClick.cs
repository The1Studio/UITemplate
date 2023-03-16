namespace TheOneStudio.UITemplate.UITemplate.ThirdPartyServices.AnalyticEvents.ABI
{
    using Core.AnalyticServices.Data;

    public class AdInterClick: IEvent
    {
        public string Placement;

        public AdInterClick(string placement)
        {
            this.Placement = placement;
        }
    }
}