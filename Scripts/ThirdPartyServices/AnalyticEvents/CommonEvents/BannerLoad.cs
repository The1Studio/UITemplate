namespace TheOneStudio.UITemplate.UITemplate.ThirdPartyServices.AnalyticEvents.CommonEvents
{
    using Core.AnalyticServices.Data;

    public class BannerLoad : IEvent
    {
        public string placement;

        public BannerLoad(string placement)
        {
            this.placement = placement;
        }
    }
}