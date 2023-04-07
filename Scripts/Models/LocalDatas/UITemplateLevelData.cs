namespace TheOneStudio.UITemplate.UITemplate.Models
{
    using System.Collections.Generic;
    using System.Linq;
    using GameFoundation.Scripts.Interfaces;

    public class UITemplateUserLevelData : ILocalData
    {
        public int                        CurrentLevel { get; set; } = 1;
        public Dictionary<int, LevelData> LevelToLevelData = new();
        public int                        LastUnlockRewardLevel;

        public void SetLevelStatusByLevel(int level, LevelData.Status status) { this.LevelToLevelData[level].LevelStatus = status; }

        public void Init()
        {
#if CREATIVE
            foreach (var levelData in this.LevelToLevelData.Values.ToList())
            {
                levelData.LevelStatus = LevelData.Status.Passed;
            }
#endif
        }
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