namespace TheOneStudio.UITemplate.UITemplate.Scripts.ThirdPartyServices.AnalyticEvents.ABI
{
    using Core.AnalyticServices.Data;

    public class AdsRewardEligible : IEvent

    {
        private readonly string place;
        public AdsRewardEligible(string place)
        {
            this.place = place;
        }
    }
}