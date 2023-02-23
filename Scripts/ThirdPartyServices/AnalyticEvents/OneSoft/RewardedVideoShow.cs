namespace TheOneStudio.UITemplate.UITemplate.ThirdPartyServices.AnalyticEvents.OneSoft
{
    using Core.AnalyticServices.Data;

    public class RewardedVideoShown : IEvent
    {
        public int    level;
        public string placement;
        
        public RewardedVideoShown(int level, string placement)
        {
            this.level     = level;
            this.placement = placement;
        }
    }
}