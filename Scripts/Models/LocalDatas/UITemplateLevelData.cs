namespace TheOneStudio.UITemplate.UITemplate.Models
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using GameFoundation.Scripts.Interfaces;
    using Newtonsoft.Json;
    using Sirenix.Serialization;
    using TheOneStudio.UITemplate.UITemplate.Blueprints;
    using TheOneStudio.UITemplate.UITemplate.Models.Controllers;
    using TheOneStudio.UITemplate.UITemplate.Models.LocalDatas;
    using UnityEngine.Scripting;

    [Preserve]
    public class UITemplateUserLevelData : ILocalData, IUITemplateLocalData
    {
        public const string ClassicMode = "classic";

        [JsonProperty] [OdinSerialize] internal UnlockType UnlockedFeature { get; set; } = UnlockType.Default;

        [JsonProperty]
        [Obsolete]
        [OdinSerialize]
        internal int CurrentLevel { get; set; } = 1;

        [JsonProperty] [OdinSerialize] internal Dictionary<string, int> ModeToCurrentLevel { get; set; } = new();
        [JsonProperty] [OdinSerialize] internal string                  CurrentMode        { get; set; } = ClassicMode;

        [JsonProperty]
        [Obsolete]
        [OdinSerialize]
        internal Dictionary<int, LevelData> LevelToLevelData { get; set; } = new();

        [JsonProperty] [OdinSerialize] internal Dictionary<string, Dictionary<int, LevelData>> ModeToLevelToLevelData { get; set; } = new();

        [JsonProperty] [OdinSerialize] internal int   LastUnlockRewardLevel;
        [JsonProperty] [OdinSerialize] internal float TimeStamp; //Time stamp in second

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
            #pragma warning disable CS0612 // Type or member is obsolete
            this.ModeToCurrentLevel.TryAdd(ClassicMode, this.CurrentLevel);
            this.ModeToLevelToLevelData.TryAdd(ClassicMode, this.LevelToLevelData);
            #pragma warning restore CS0612 // Type or member is obsolete
        }

        public Type ControllerType => typeof(UITemplateLevelDataController);
    }

    public class LevelData
    {
        [JsonProperty] public int    Level       { get; internal set; }
        [JsonProperty] public Status LevelStatus { get; internal set; }
        [JsonProperty] public int    StarCount   { get; internal set; }
        [JsonProperty] public int    LoseCount   { get; internal set; }
        [JsonProperty] public int    WinCount    { get; internal set; }

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
            Unlocked
        }
    }
}