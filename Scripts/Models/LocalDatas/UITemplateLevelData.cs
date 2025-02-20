namespace TheOneStudio.UITemplate.UITemplate.Models
{
    using System;
    using System.Collections.Generic;
    using GameFoundation.Scripts.Interfaces;
    using GameFoundation.Scripts.Utilities.Extension;
    using Sirenix.Serialization;
    using TheOneStudio.UITemplate.UITemplate.Models.Controllers;
    using TheOneStudio.UITemplate.UITemplate.Models.LocalDatas;
    using UnityEngine.Scripting;

    [Preserve]
    public class UITemplateUserLevelData : ILocalData, IUITemplateLocalData
    {
        public const string ClassicMode = "classic";
        
        [OdinSerialize] public UITemplateItemData.UnlockType UnlockedFeature { get; set; } = UITemplateItemData.UnlockType.Default;

        [OdinSerialize] public int CurrentLevel { get; set; } = 1;

        [OdinSerialize] public Dictionary<string, int> ModeToCurrentLevel { get; set; } = new();

        [OdinSerialize] public Dictionary<int, LevelData> LevelToLevelData { get; set; } = new();

        [OdinSerialize] public Dictionary<string, Dictionary<int, LevelData>> ModeToLevelToLevelData { get; set; } = new();

        [OdinSerialize] public int LastUnlockRewardLevel;

        public void Init()
        {
#if CREATIVE
            foreach (var levelData in this.LevelToLevelData.Values.ToList())
            {
                levelData.LevelStatus = LevelData.Status.Passed;
            }
#endif
        }

        public void OnDataLoaded()
        {
            this.ModeToCurrentLevel.GetOrAdd(ClassicMode, () => this.CurrentLevel);
            this.ModeToLevelToLevelData.GetOrAdd(ClassicMode, () => this.LevelToLevelData); 
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