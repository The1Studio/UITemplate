namespace TheOneStudio.UITemplate.UITemplate.ThirdPartyServices.AnalyticEvents.ABI
{
    using Core.AnalyticServices.Data;

    public class AdsRewardShow: IEvent
    {
        public string Placement;

        public AdsRewardShow(string placement)
        {
            this.Placement = placement;
        }
    }
}