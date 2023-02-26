namespace TheOneStudio.UITemplate.UITemplate.ThirdPartyServices.AnalyticEvents.Wido
{
    using Core.AnalyticServices.Data;

    public class LevelFailed : IEvent
    {
        public int level;
        public int time_spent;
        public LevelFailed(int level, int timeSpent)
        {
            this.level      = level;
            this.time_spent = timeSpent;
        }
    }
}