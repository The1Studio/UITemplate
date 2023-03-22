namespace TheOneStudio.UITemplate.UITemplate.Models
{
    using System.Collections.Generic;
    using GameFoundation.Scripts.Interfaces;

    public class UITemplateUserLevelData : ILocalData
    {
        internal int                        CurrentLevel     { get; set; } = 1;
        internal Dictionary<int, LevelData> LevelToLevelData { get; set; } = new();

        public void Init() { }
    }

    public class LevelData
    {
        public int    Level;
        public Status LevelStatus;
        public int    StarCount;
        public int    LoseCount;
        public int    WinCount;

        public LevelData(int level, Status levelStatus, int loseCount = 0, int winCount = 0, int starCount = 0)
        {
            this.Level       = level;
            this.LevelStatus = levelStatus;
            this.LoseCount   = loseCount;
            this.WinCount    = winCount;
            this.StarCount   = starCount;
        }

        public enum Status
        {
            Locked,
            Passed,
            Skipped,
        }
    }
}