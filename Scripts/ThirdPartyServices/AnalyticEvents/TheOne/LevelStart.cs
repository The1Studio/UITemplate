namespace TheOneStudio.UITemplate.UITemplate.ThirdPartyServices.AnalyticEvents.TheOne
{
    using Core.AnalyticServices.Data;

    public class LevelStart : IEvent
    {
        public int  Level                 { get; set; }
        public int  Gold                  { get; set; }
        public int  TotalLevelsPlayed     { get; set; }
        public long Timestamp             { get; set; }
        public int  GameModeId            { get; set; }
        public int  TotalLevelsTypePlayed { get; set; }

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
}