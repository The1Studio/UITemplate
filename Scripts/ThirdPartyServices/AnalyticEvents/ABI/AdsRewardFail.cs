namespace TheOneStudio.UITemplate.UITemplate.ThirdPartyServices.AnalyticEvents.ABI
{
    using Core.AnalyticServices.Data;

    public class AdsRewardFail: IEvent
    {
        public string Placement;
        public string Message;

        public AdsRewardFail(string placement, string message)
        {
            this.Placement   = placement;
            this.Message = message;
        }
    }
}