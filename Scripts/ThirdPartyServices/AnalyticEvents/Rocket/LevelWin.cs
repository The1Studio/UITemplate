namespace TheOneStudio.UITemplate.UITemplate.ThirdPartyServices.AnalyticEvents.Rocket
{
    using Core.AnalyticServices.Data;

    public class LevelWin : IEvent
    {
        public int level;
        public int time;
        public LevelWin(int level, int time)
        {
            this.level = level;
            this.time  = time;
        }
    }
}