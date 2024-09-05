namespace TheOneStudio.UITemplate.UITemplate.ThirdPartyServices.AnalyticEvents.Mirai
{
    using Core.AnalyticServices.Data;

    public class AdsBannerFail : IEvent
    {
        public string errormsg;

        public AdsBannerFail(string errormsg)
        {
            this.errormsg = errormsg;
        }
    }
}