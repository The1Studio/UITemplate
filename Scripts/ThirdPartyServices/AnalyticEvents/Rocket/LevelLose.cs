namespace TheOneStudio.UITemplate.UITemplate.ThirdPartyServices.AnalyticEvents.Rocket
{
    using Core.AnalyticServices.Data;

    public class LevelLose : IEvent
    {
        public int level;
        public int time;
        
        public LevelLose(int level, int time)
        {
            this.level = level;
            this.time  = time;
        }
    }
}