namespace TheOneStudio.UITemplate.UITemplate.ThirdPartyServices.AnalyticEvents.ABI.Firebase
{
    using Core.AnalyticServices.Data;

    public class AdsRewardOffer: IEvent
    {
        public string Placement;

        public AdsRewardOffer(string placement)
        {
            this.Placement = placement;
        }
    }
}