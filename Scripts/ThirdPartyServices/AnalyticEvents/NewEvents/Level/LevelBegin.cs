namespace TheOneStudio.UITemplate.ThirdPartyServices.AnalyticEvents.Level
{
    using Core.AnalyticServices.Data;

    public class LevelBegin : IEvent
    {
        public int  Level                 { get; set; }
        public int  TotalLevelsPlayed     { get; set; }
        public long Timestamp             { get; set; }
        public int  GameModeId            { get; set; }
        public int  TotalLevelsTypePlayed { get; set; }

        public LevelBegin(int level, int totalLevelsPlayed, long timestamp, int gameModeId, int totalLevelsTypePlayed)
        {
            this.Level                 = level;
            this.TotalLevelsPlayed     = totalLevelsPlayed;
            this.Timestamp             = timestamp;
            this.GameModeId            = gameModeId;
            this.TotalLevelsTypePlayed = totalLevelsTypePlayed;
        }
    }
}