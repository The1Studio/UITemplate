namespace TheOneStudio.UITemplate.UITemplate.ThirdPartyServices.AnalyticEvents.ABI
{
    using Core.AnalyticServices.Data;

    public class LevelComplete : IEvent
    {
        public int Level;
        public int TimeSpent;
        public int Session;

        public LevelComplete(int level, int timeSpent, int session)
        {
            this.Level     = level;
            this.TimeSpent = timeSpent;
            this.Session   = session;
        }
    }

    public class LevelAchieved : IEvent
    {
        public int Level;
        public int TimeSpent;

        public LevelAchieved(int level, int timeSpent)
        {
            this.Level     = level;
            this.TimeSpent = timeSpent;
        }
    }
}