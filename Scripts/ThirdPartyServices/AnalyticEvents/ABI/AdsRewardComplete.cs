namespace TheOneStudio.UITemplate.UITemplate.Scripts.ThirdPartyServices.AnalyticEvents.ABI
{
    using Core.AnalyticServices.Data;

    public class AdsRewardComplete : IEvent
    {
        public string Placement;

        public AdsRewardComplete(string placement)
        {
            this.Placement = placement;
        }
    }
}