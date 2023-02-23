namespace TheOneStudio.UITemplate.UITemplate.ThirdPartyServices.AnalyticEvents.OneSoft
{
    using Core.AnalyticServices.Data;

    public class LevelStart : IEvent
    {
        public int level;
        public LevelStart(int level) { this.level = level; }
    }
}