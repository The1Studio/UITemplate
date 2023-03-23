namespace TheOneStudio.UITemplate.UITemplate.Scripts.ThirdPartyServices.AnalyticEvents.AdOne
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