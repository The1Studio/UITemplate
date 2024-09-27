using System.Linq;

namespace TheOneStudio.UITemplate.UITemplate.Models
{
    using System;
    using System.Collections.Generic;
    using GameFoundation.Scripts.Interfaces;
    using Sirenix.Serialization;
    using TheOneStudio.UITemplate.UITemplate.Models.Controllers;
    using TheOneStudio.UITemplate.UITemplate.Models.LocalDatas;
    using UnityEngine.Scripting;

    [Preserve]
    public class UITemplateUserLevelData : ILocalData, IUITemplateLocalData
    {
        [OdinSerialize] public UITemplateItemData.UnlockType UnlockedFeature { get; set; } = UITemplateItemData.UnlockType.Default;

        [OdinSerialize] public int CurrentLevel { get; set; } = 1;

        [OdinSerialize] public Dictionary<int, LevelData> LevelToLevelData { get; set; } = new();

        [OdinSerialize] public int LastUnlockRewardLevel;

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

        public Type ControllerType => typeof(UITemplateLevelDataController);
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