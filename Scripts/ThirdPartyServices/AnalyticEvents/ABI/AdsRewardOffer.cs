namespace TheOneStudio.UITemplate.UITemplate.Scripts.ThirdPartyServices.AnalyticEvents.ABI
{
    using Core.AnalyticServices.Data;

    public class AdsRewardOffer : IEvent
    {
        public string Placement;

        public AdsRewardOffer(string placement)
        {
            this.Placement = placement;
        }
    }
}