namespace TheOneStudio.UITemplate.UITemplate.ThirdPartyServices.AnalyticEvents.CommonEvents
{
    using Core.AnalyticServices.Data;

    public class LevelStart : IEvent
    {
        public int  level;
        public int  gold;
        public int  worldId;
        public long timeStamp;

        public LevelStart(int level, int gold, int worldId = 0, long? timeStamp = null)
        {
            this.level   = level;
            this.gold    = gold;
            this.worldId = worldId;
            if (timeStamp != null) this.timeStamp = (long)timeStamp;
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

    public class LevelEnd : IEvent
    {
        public int    level;
        public string status;
        public int    worldId;
        public double timePlay;
        public long   timerStamp;

        public LevelEnd(int level, string status, int worldId, double timePlay, long timeStamp)
        {
            this.level      = level;
            this.status     = status;
            this.worldId    = worldId;
            this.timePlay   = timePlay;
            this.timerStamp = timeStamp;
        }
    }

    public class CompleteLevel1 : IEvent
    {
    }
    public class CompleteLevel5 : IEvent
    {
    }
    public class CompleteLevel10 : IEvent
    {
    }
}