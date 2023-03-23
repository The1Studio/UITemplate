namespace TheOneStudio.UITemplate.UITemplate.Scripts.ThirdPartyServices.AnalyticEvents.AdOne
{
    using Core.AnalyticServices.Data;

    public class LevelPassed : IEvent
    {
        public int level;
        public int time_spent;
        
        public LevelPassed(int level, int timeSpent)
        {
            this.level      = level;
            this.time_spent = timeSpent;
        }
    }
}