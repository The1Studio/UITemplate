namespace TheOneStudio.UITemplate.UITemplate.ThirdPartyServices.AnalyticEvents.Rocket
{
    using Core.AnalyticServices.Data;

    public class LevelSkipped : IEvent
    {
        public int level;
        public int time;
        public LevelSkipped(int level, int time)
        {
            this.level = level;
            this.time  = time;
        }
    }
}