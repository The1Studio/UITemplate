namespace TheOneStudio.UITemplate.UITemplate.ThirdPartyServices.AnalyticEvents.CommonEvents
{
    using Core.AnalyticServices.Data;

    public class BannerLoadFail : IEvent
    {
        public string errormsg;

        public BannerLoadFail(string errormsg)
        {
            this.errormsg  = errormsg;
        }
    }
}