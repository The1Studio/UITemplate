namespace TheOneStudio.UITemplate.UITemplate.ThirdPartyServices.AnalyticEvents.Rocket
{
    using Core.AnalyticServices.Data;

    public class InterstitialShow : IEvent
    {
        public int    level;
        public string placement;
        public InterstitialShow(int level, string placement)
        {
            this.level     = level;
            this.placement = placement;
        }
    }
}