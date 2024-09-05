namespace TheOneStudio.UITemplate.UITemplate.ThirdPartyServices.AnalyticEvents.CommonEvents
{
    using Core.AnalyticServices.Data;

    public class BannerLoadFail : IEvent
    {
        public string placement;
        public string errormsg;

        public BannerLoadFail(string placement, string errormsg)
        {
            this.placement = placement;
            this.errormsg  = errormsg;
        }
    }
}