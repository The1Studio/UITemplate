namespace TheOneStudio.UITemplate.UITemplate.ThirdPartyServices.AnalyticEvents.ABI.Firebase
{
    using Core.AnalyticServices.Data;

    public class LevelSkipped : IEvent
    {
        public int Level;
        public int Time;

        public LevelSkipped(int level, int time)
        {
            this.Level = level;
            this.Time  = time;
        }
    }
}