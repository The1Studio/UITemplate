namespace TheOneStudio.UITemplate.UITemplate.ThirdPartyServices.AnalyticEvents.Rocket
{
    using Core.AnalyticServices.Data;

    public class RewardedAdsShowCompleted : IEvent
    {
        public string placement;
        public string result;
        
        public RewardedAdsShowCompleted(string placement, string result)
        {
            this.placement = placement;
            this.result    = result;
        }
    }
}