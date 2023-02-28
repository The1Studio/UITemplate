namespace TheOneStudio.UITemplate.UITemplate.Models
{
    using System.Collections.Generic;
    using GameFoundation.Scripts.Interfaces;

    public class UITemplateUserLevelData : ILocalData
    {
        public int                        CurrentLevel { get; set; } = 1;
        public Dictionary<int, LevelData> LevelToLevelData = new();

        public void Init() { }
    }

    public class LevelData
    {
        public int    Level;
        public Status LevelStatus;
        public int    StarCount;

        public LevelData(int level, Status levelStatus, int starCount = 0)
        {
            this.Level       = level;
            this.LevelStatus = levelStatus;
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