namespace TheOneStudio.UITemplate.UITemplate.ThirdPartyServices.AnalyticEvents.ABI.Firebase
{
    using Core.AnalyticServices.Data;

    public class AdsRewardClick: IEvent
    {
        public string Placement;

        public AdsRewardClick(string placement) { this.Placement = placement; }
    }
}