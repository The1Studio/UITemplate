namespace TheOneStudio.UITemplate.UITemplate.Scripts.ThirdPartyServices.AnalyticEvents.ABI
{
    using Core.AnalyticServices.Data;

    public class AdsRewardClick : IEvent
    {
        public string Placement;

        public AdsRewardClick(string placement)
        {
            this.Placement = placement;
        }
    }
}