namespace TheOneStudio.UITemplate.UITemplate.ThirdPartyServices.AnalyticEvents.CommonEvents
{
    using System;
    using Core.AnalyticServices.Data;

    /// <summary> Event for level start.</summary>
    public class LevelStart : IEvent
    {
        /// <summary> Level of the event.</summary>
        public int  Level                 { get; set; }
        
        public int  Gold                  { get; set; }
        
        /// <summary> Total level played.</summary>
        public int  TotalLevelsPlayed     { get; set; }
        
        /// <summary> Timestamp for the event.</summary>
        public long Timestamp             { get; set; }
        
        /// <summary> Game mode id of the level.</summary>
        public int  GameModeId            { get; set; }
        
        /// <summary> Total levels played in the same game mode.</summary>
        public int  TotalLevelsTypePlayed { get; set; }

        [Obsolete]
        public LevelStart(int level, int gold)
        {
            this.Level = level;
            this.Gold  = gold;
        }
        
        public LevelStart(int level, int gold, int totalLevelsPlayed, long timestamp, int gameModeId, int totalLevelsTypePlayed)
        {
            this.Level                 = level;
            this.Gold                  = gold;
            this.GameModeId            = gameModeId;
            this.Timestamp             = timestamp;
            this.TotalLevelsPlayed     = totalLevelsPlayed;
            this.TotalLevelsTypePlayed = totalLevelsTypePlayed;
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