namespace TheOneStudio.UITemplate.UITemplate.Scripts.ThirdPartyServices.AnalyticEvents.AdOne
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