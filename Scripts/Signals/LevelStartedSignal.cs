namespace TheOneStudio.UITemplate.UITemplate.Signals
{
    public class LevelStartedSignal
    {
        public int  Level                 { get; private set; }
        public int  TotalLevelsPlayed     { get; private set; }
        public long Timestamp             { get; private set; }
        public int  GameModeId            { get; private set; }
        public int  TotalLevelsTypePlayed { get; private set; }

        public LevelStartedSignal(int level, int totalLevelsPlayed, long timestamp, int gameModeId = 0, int totalLevelsTypePlayed = 0)
        {
            this.Level                 = level;
            this.TotalLevelsPlayed     = totalLevelsPlayed;
            this.Timestamp             = timestamp;
            this.GameModeId            = gameModeId;
            this.TotalLevelsTypePlayed = totalLevelsTypePlayed;
        }
    }
}