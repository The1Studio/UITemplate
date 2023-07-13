namespace TheOneStudio.UITemplate.UITemplate.ThirdPartyServices.AnalyticEvents.CommonEvents
{
    using Core.AnalyticServices.Data;

    public class LevelStart : IEvent
    {
        public int level;
        public int gold;
        
        public LevelStart(int level, int gold)
        {
            this.level = level;
            this.gold  = gold;
        }
    }
    
    public class LevelWin : IEvent
    {
        public int level;
        public int timeSpent;

        public LevelWin(int level, int timeSpent)
        {
            this.level     = level;
            this.timeSpent = timeSpent;
        }
    }
    
    public class LevelLose : IEvent
    {
        public int level;
        public int timeSpent;
        
        public LevelLose(int level, int timeSpent)
        {
            this.level     = level;
            this.timeSpent = timeSpent;
        }
    }
    
    public class FirstWin : IEvent
    {
        public int level;
        public int timeSpent;
        
        public FirstWin(int level, int timeSpent)
        {
            this.level     = level;
            this.timeSpent = timeSpent;
        }
    }
    
    public class LevelSkipped : IEvent
    {
        public int level;
        public int timeSpent;
        
        public LevelSkipped(int level, int timeSpent)
        {
            this.level     = level;
            this.timeSpent = timeSpent;
        }
    }
}