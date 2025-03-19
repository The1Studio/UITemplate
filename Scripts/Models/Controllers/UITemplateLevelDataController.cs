namespace TheOneStudio.UITemplate.UITemplate.Models.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using GameFoundation.Scripts.Utilities.Extension;
    using GameFoundation.Scripts.Utilities.UserData;
    using GameFoundation.Signals;
    using TheOneStudio.UITemplate.UITemplate.Blueprints;
    using TheOneStudio.UITemplate.UITemplate.Signals;
    using UnityEngine.Scripting;

    public class UITemplateLevelDataController : IUITemplateControllerData
    {
        private Dictionary<string, Dictionary<int, DateTime>> modeToLevelToStartLevelTime = new();

        [Preserve]
        public UITemplateLevelDataController(UITemplateLevelBlueprint uiTemplateLevelBlueprint, UITemplateUserLevelData uiTemplateUserLevelData,
            UITemplateInventoryDataController uiTemplateInventoryDataController, SignalBus signalBus, IHandleUserDataServices handleUserDataServices)
        {
            this.uiTemplateLevelBlueprint          = uiTemplateLevelBlueprint;
            this.uiTemplateUserLevelData           = uiTemplateUserLevelData;
            this.uiTemplateInventoryDataController = uiTemplateInventoryDataController;
            this.signalBus                         = signalBus;
            this.handleUserDataServices            = handleUserDataServices;
        }

        public UnlockType UnlockedFeature => this.uiTemplateUserLevelData.UnlockedFeature;

        public int LastUnlockRewardLevel { get => this.uiTemplateUserLevelData.LastUnlockRewardLevel; set => this.uiTemplateUserLevelData.LastUnlockRewardLevel = value; }

        public int TotalLose => this.GetModelData().Values.Sum(levelData => levelData.LoseCount);
        public int TotalWin  => this.GetModelData().Values.Sum(levelData => levelData.WinCount);

        public LevelData GetCurrentLevelData                  => this.GetLevelData(this.CurrentLevel);
        public int       CurrentLevel                         => this.CurrentModeLevel(UITemplateUserLevelData.ClassicMode);
        public LevelData GetCurrentModeLevelData(string mode) => this.GetLevelData(this.CurrentLevel, mode);

        public int CurrentModeLevel(string mode) => this.uiTemplateUserLevelData.ModeToCurrentLevel.GetOrAdd(mode, () => 1);

        public int MaxLevel
        {
            get
            {
                var levelDataList = this.GetModelData().Values.Where(levelData => levelData.LevelStatus == LevelData.Status.Passed).ToList();

                return levelDataList.Count == 0 ? 0 : levelDataList.Max(data => data.Level);
            }
        }

        public int TotalLevelSurpassed
        {
            get
            {
                var levelDatas = this.GetModelData().Values.Where(levelData => levelData.LevelStatus != LevelData.Status.Locked).ToList();

                return levelDatas.Count == 0 ? 0 : levelDatas.Max(data => data.Level);
            }
        }

        public bool IsFeatureUnlocked(UnlockType feature) { return (this.uiTemplateUserLevelData.UnlockedFeature & feature) != 0; }

        public void UnlockFeature(UnlockType feature) { this.uiTemplateUserLevelData.UnlockedFeature |= feature; }

        public List<LevelData> GetAllLevels() { return this.uiTemplateLevelBlueprint.Values.Select(levelRecord => this.GetLevelData(levelRecord.Level)).ToList(); }

        public IEnumerable<LevelData> GetAllLevelData(string mode = UITemplateUserLevelData.ClassicMode) { return this.GetModelData(mode).Values; }

        public LevelData GetLevelData(int level, string mode = UITemplateUserLevelData.ClassicMode)
        {
            return this.uiTemplateUserLevelData.ModeToLevelToLevelData.GetOrAdd(mode, () => new Dictionary<int, LevelData>()).GetOrAdd(level, () => new LevelData(level, LevelData.Status.Locked));
        }

        private Dictionary<int, LevelData> GetModelData(string mode = UITemplateUserLevelData.ClassicMode)
        {
            return this.uiTemplateUserLevelData.ModeToLevelToLevelData.GetOrAdd(mode, () => new Dictionary<int, LevelData>());
        }

        private DateTime GetModePlayTime(string mode, int level) => this.modeToLevelToStartLevelTime.GetOrAdd(mode, () => new Dictionary<int, DateTime>()).GetOrAdd(level, () => DateTime.UtcNow);

        /// <summary>Have be called when level started</summary>
        public void PlayCurrentLevel(string mode = UITemplateUserLevelData.ClassicMode)
        {
            var currentModeLevel = this.CurrentModeLevel(mode);
            this.GetModePlayTime(mode, currentModeLevel);
            this.signalBus.Fire(new LevelStartedSignal(currentModeLevel, mode));
        }

        /// <summary>
        /// Called when select a level in level selection screen
        /// </summary>
        /// <param name="level">selected level</param>
        /// <param name="mode">the mode of level</param>
        public void SelectLevel(int level, string mode = UITemplateUserLevelData.ClassicMode)
        {
            this.uiTemplateUserLevelData.ModeToCurrentLevel[mode] = level;

            this.handleUserDataServices.SaveAll();
        }

        private int GetCurrentLevelPlayTime(string mode = UITemplateUserLevelData.ClassicMode)
        {
            var currentModeLevel = this.CurrentModeLevel(mode);
            var startTime        = this.modeToLevelToStartLevelTime.GetOrAdd(mode, () => new Dictionary<int, DateTime>()).GetOrAdd(currentModeLevel, () => DateTime.UtcNow);

            return (int)(DateTime.UtcNow - startTime).TotalSeconds;
        }

        /// <summary>
        /// Called when player lose current level
        /// </summary>
        public void LoseCurrentLevel(string mode = UITemplateUserLevelData.ClassicMode)
        {
            var currentModeLevel = this.CurrentModeLevel(mode);
            this.signalBus.Fire(new LevelEndedSignal(currentModeLevel, mode, false, this.GetCurrentLevelPlayTime(), null));
            this.GetLevelData(currentModeLevel, mode).LoseCount++;

            this.handleUserDataServices.SaveAll();
        }

        /// <summary>
        /// Called when player win current level
        /// </summary>
        public void PassCurrentLevel(string mode = UITemplateUserLevelData.ClassicMode, int starCount = 1)
        {
            var currentModeLevel = this.CurrentModeLevel(mode);
            var levelData        = this.GetLevelData(currentModeLevel, mode);
            levelData.WinCount++;
            levelData.LevelStatus = LevelData.Status.Passed;
            levelData.StarCount   = starCount;
            this.signalBus.Fire(new LevelEndedSignal(currentModeLevel, mode, true, this.GetCurrentLevelPlayTime(), null));
            this.uiTemplateUserLevelData.ModeToCurrentLevel[mode]++;

            this.handleUserDataServices.SaveAll();
        }

        /// <summary>
        /// Called when player skip current level
        /// </summary>
        /// <param name="mode">Play mode</param>
        public void SkipCurrentLevel(string mode = UITemplateUserLevelData.ClassicMode)
        {
            var currentModeLevel = this.CurrentModeLevel(mode);
            var levelData        = this.GetLevelData(currentModeLevel, mode);
            if (levelData.LevelStatus == LevelData.Status.Locked)
            {
                levelData.LevelStatus = LevelData.Status.Skipped;
            }

            this.signalBus.Fire(new LevelSkippedSignal(currentModeLevel, mode, this.GetCurrentLevelPlayTime(mode)));
            this.uiTemplateUserLevelData.ModeToCurrentLevel[mode]++;

            this.handleUserDataServices.SaveAll();
        }

        public bool IsTouchedLevel(int level) { return this.MaxLevel + 1 >= level; }

        public bool CheckLevelIsUnlockedStatus(int level, string mode = UITemplateUserLevelData.ClassicMode)
        {
            var skippedLevel      = this.GetModelData(mode).Values.LastOrDefault(levelData => levelData.LevelStatus == LevelData.Status.Skipped);
            var skippedLevelIndex = this.GetModelData(mode).Values.ToList().IndexOf(skippedLevel);
            if (skippedLevelIndex == -1 && this.MaxLevel == 0 && level == 1) return true;

            var maxIndex = Math.Max(skippedLevelIndex, this.MaxLevel);

            return level == maxIndex + 1;
        }

        public float GetRewardProgress(int level)
        {
            var levelUnlockReward = this.GetLevelUnlockReward(level);
            if (levelUnlockReward < 0) return 0;

            //update last unlock reward level
            var temp = this.GetLastLevelUnlockReward(level);

            return (float)(level - temp) / (levelUnlockReward - temp);
        }

        public string GetRewardId(int currentLevel)
        {
            var levelUnlockReward = this.GetLevelUnlockReward(currentLevel);

            if (levelUnlockReward < 0) return null;
            var rewardList = this.uiTemplateLevelBlueprint.GetDataById(levelUnlockReward).Rewards;

            for (var i = 0; i < rewardList.Count; i++)
                if (this.uiTemplateInventoryDataController.GetItemData(rewardList[i]).CurrentStatus != Status.Owned)
                    return rewardList[i];

            return null;
        }

        public Dictionary<string, int> GetAltReward(int currentLevel)
        {
            var levelUnlockReward = this.GetLevelUnlockReward(currentLevel);

            return levelUnlockReward < 0 ? null : this.uiTemplateLevelBlueprint.GetDataById(levelUnlockReward).AltReward;
        }

        private int GetLevelUnlockReward(int level)
        {
            for (var i = level; i <= this.uiTemplateLevelBlueprint.Count; i++)
                if (this.uiTemplateLevelBlueprint.GetDataById(i).Rewards.Count > 0)
                    return i;

            return -1;
        }

        private int GetLastLevelUnlockReward(int level)
        {
            for (var i = level - 1; i > 0; i--)
                if (this.uiTemplateLevelBlueprint.GetDataById(i).Rewards.Count > 0)
                    return i;

            return 0;
        }

        #region inject

        private readonly UITemplateLevelBlueprint          uiTemplateLevelBlueprint;
        private readonly UITemplateUserLevelData           uiTemplateUserLevelData;
        private readonly UITemplateInventoryDataController uiTemplateInventoryDataController;
        private readonly SignalBus                         signalBus;
        private readonly IHandleUserDataServices           handleUserDataServices;

        #endregion
    }
}