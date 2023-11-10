namespace TheOneStudio.UITemplate.UITemplate.Scripts.ThirdPartyServices.AnalyticEvents.AdOne
{
    using Core.AnalyticServices.Data;

    public class AdsRewardedCalled : IEvent
    {
        public string Placement;

        public AdsRewardedCalled(string placement) { this.Placement = placement; }
    }
}