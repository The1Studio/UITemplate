namespace TheOneStudio.UITemplate.UITemplate.ThirdPartyServices.AnalyticEvents.Wido
{
    using Core.AnalyticServices.Data;

    public class RewardedAdsSuccess : IEvent
    {
        public string placement;
        public string result;
        
        public RewardedAdsSuccess(string placement, string result)
        {
            this.placement = placement;
            this.result    = result;
        }
    }
}