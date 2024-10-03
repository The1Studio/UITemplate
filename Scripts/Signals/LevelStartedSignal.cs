namespace TheOneStudio.UITemplate.UITemplate.Signals
{
    public class LevelStartedSignal
    {
        public int  Level                 { get; private set; }
        public int  TotalLevelsPlayed     { get; private set; }
        public long TimeBetweenLastSignal { get; private set; }
        public int  GameModeId            { get; private set; }
        public int  TotalLevelsTypePlayed { get; private set; }

        public LevelStartedSignal(int level, int totalLevelsPlayed = 0, long timeBetweenLastSignal = 0, int gameModeId = 0, int totalLevelsTypePlayed = 0)
        {
            this.Level                 = level;
            this.TotalLevelsPlayed     = totalLevelsPlayed;
            this.TimeBetweenLastSignal = timeBetweenLastSignal;
            this.GameModeId            = gameModeId;
            this.TotalLevelsTypePlayed = totalLevelsTypePlayed;
        }
    }
}