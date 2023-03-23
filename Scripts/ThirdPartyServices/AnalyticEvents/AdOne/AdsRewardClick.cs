namespace TheOneStudio.UITemplate.UITemplate.Scripts.ThirdPartyServices.AnalyticEvents.AdOne
{
    using Core.AnalyticServices.Data;

    public class AdsRewardClick: IEvent
    {
        public string Placement;

        public AdsRewardClick(string placement) { this.Placement = placement; }
    }
}