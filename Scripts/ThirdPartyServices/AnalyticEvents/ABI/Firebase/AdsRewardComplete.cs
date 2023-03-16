namespace TheOneStudio.UITemplate.UITemplate.ThirdPartyServices.AnalyticEvents.ABI.Firebase
{
    using Core.AnalyticServices.Data;

    public class AdsRewardComplete : IEvent
    {
        public string Placement;

        public AdsRewardComplete(string placement) { this.Placement = placement; }
    }
}