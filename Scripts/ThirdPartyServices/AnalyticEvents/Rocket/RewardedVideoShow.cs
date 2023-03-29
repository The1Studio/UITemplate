namespace TheOneStudio.UITemplate.UITemplate.ThirdPartyServices.AnalyticEvents.Rocket
{
    using Core.AnalyticServices.Data;

    public class RewardedVideoShow : IEvent
    {
        public int    level;
        public string placement;
        
        public RewardedVideoShow(int level, string placement)
        {
            this.level     = level;
            this.placement = placement;
        }
    }
}