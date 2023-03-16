namespace TheOneStudio.UITemplate.UITemplate.ThirdPartyServices.AnalyticEvents.ABI
{
    using Core.AnalyticServices.Data;

    public class AdInterFail: IEvent
    {
        public string errormsg;

        public AdInterFail(string pErrormsg)
        {
            this.errormsg = pErrormsg;
        }
    }
}