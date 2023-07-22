namespace TheOneStudio.UITemplate.UITemplate.Scripts.ThirdPartyServices.AnalyticEvents.ABI
{
    using Core.AnalyticServices.Data;

    public class AdsRewardEligible : IEvent

    {
        private readonly string placement;
        public AdsRewardEligible(string placement)
        {
            this.placement = placement;
        }
    }
}