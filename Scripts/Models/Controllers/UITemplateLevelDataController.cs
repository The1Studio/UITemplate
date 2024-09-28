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
        #region inject

        private readonly UITemplateLevelBlueprint          uiTemplateLevelBlueprint;
        private readonly UITemplateUserLevelData           uiTemplateUserLevelData;
        private readonly UITemplateInventoryDataController uiTemplateInventoryDataController;
        private readonly SignalBus                         signalBus;
        private readonly IHandleUserDataServices           handleUserDataServices;

        #endregion

        [Preserve]
        public UITemplateLevelDataController(UITemplateLevelBlueprint uiTemplateLevelBlueprint, UITemplateUserLevelData uiTemplateUserLevelData, UITemplateInventoryDataController uiTemplateInventoryDataController, SignalBus signalBus, IHandleUserDataServices handleUserDataServices)
        {
            this.uiTemplateLevelBlueprint          = uiTemplateLevelBlueprint;
            this.uiTemplateUserLevelData           = uiTemplateUserLevelData;
            this.uiTemplateInventoryDataController = uiTemplateInventoryDataController;
            this.signalBus                         = signalBus;
            this.handleUserDataServices            = handleUserDataServices;
        }

        public UITemplateItemData.UnlockType UnlockedFeature => this.uiTemplateUserLevelData.UnlockedFeature;

        public bool IsFeatureUnlocked(UITemplateItemData.UnlockType feature) => (this.uiTemplateUserLevelData.UnlockedFeature & feature) != 0;

        public void UnlockFeature(UITemplateItemData.UnlockType feature) => this.uiTemplateUserLevelData.UnlockedFeature |= feature;

        public int LastUnlockRewardLevel
        {
            get => this.uiTemplateUserLevelData.LastUnlockRewardLevel;
            set => this.uiTemplateUserLevelData.LastUnlockRewardLevel = value;
        }

        public List<LevelData> GetAllLevels()
        {
            return this.uiTemplateLevelBlueprint.Values.Select(levelRecord => this.GetLevelData(levelRecord.Level)).ToList();
        }

        public LevelData GetLevelData(int level)
        {
            return this.uiTemplateUserLevelData.LevelToLevelData.GetOrAdd(level, () => new LevelData(level, LevelData.Status.Locked));
        }

        /// <summary>Have be called when level started</summary>
        public void PlayCurrentLevel()
        {
            this.signalBus.Fire(new LevelStartedSignal(this.uiTemplateUserLevelData.CurrentLevel));
        }

        /// <summary>
        /// Called when select a level in level selection screen
        /// </summary>
        /// <param name="level">selected level</param>
        public void SelectLevel(int level)
        {
            this.uiTemplateUserLevelData.CurrentLevel = level;

            this.handleUserDataServices.SaveAll();
        }

        /// <summary>
        /// Called when player lose current level
        /// </summary>
        /// <param name="time">Play time in seconds</param>
        public void LoseCurrentLevel(int time = 0)
        {
            this.signalBus.Fire(new LevelEndedSignal { Level = this.uiTemplateUserLevelData.CurrentLevel, IsWin = false, Time = time, CurrentIdToValue = null });
            this.GetLevelData(this.uiTemplateUserLevelData.CurrentLevel).LoseCount++;

            this.handleUserDataServices.SaveAll();
        }

        public int TotalLose => this.uiTemplateUserLevelData.LevelToLevelData.Values.Sum(levelData => levelData.LoseCount);
        public int TotalWin => this.uiTemplateUserLevelData.LevelToLevelData.Values.Sum(levelData => levelData.WinCount);

        /// <summary>
        /// Called when player win current level
        /// </summary>
        /// <param name="time">Play time in seconds</param>
        public void PassCurrentLevel(int time = 0)
        {
            this.GetLevelData(this.uiTemplateUserLevelData.CurrentLevel).WinCount++;
            this.uiTemplateUserLevelData.SetLevelStatusByLevel(this.uiTemplateUserLevelData.CurrentLevel, LevelData.Status.Passed);
            this.signalBus.Fire(new LevelEndedSignal { Level = this.uiTemplateUserLevelData.CurrentLevel, IsWin = true, Time = time, CurrentIdToValue = null });
            if(GetCurrentLevelData.LevelStatus == LevelData.Status.Locked) this.uiTemplateUserLevelData.SetLevelStatusByLevel(this.uiTemplateUserLevelData.CurrentLevel, LevelData.Status.Passed);
            this.uiTemplateUserLevelData.CurrentLevel++;

            this.handleUserDataServices.SaveAll();
        }

        /// <summary>
        /// Called when player skip current level
        /// </summary>
        /// <param name="time">Play time in seconds</param>
        public void SkipCurrentLevel(int time = 0)
        {
            if(GetCurrentLevelData.LevelStatus == LevelData.Status.Locked) this.uiTemplateUserLevelData.SetLevelStatusByLevel(this.uiTemplateUserLevelData.CurrentLevel, LevelData.Status.Skipped);
            this.signalBus.Fire(new LevelSkippedSignal { Level = this.uiTemplateUserLevelData.CurrentLevel, Time = time });
            this.uiTemplateUserLevelData.CurrentLevel++;

            this.handleUserDataServices.SaveAll();
        }

        public LevelData GetCurrentLevelData => this.GetLevelData(this.uiTemplateUserLevelData.CurrentLevel);
        public int       CurrentLevel        => this.GetLevelData(this.uiTemplateUserLevelData.CurrentLevel).Level;

        public int MaxLevel
        {
            get
            {
                var levelDatas = this.uiTemplateUserLevelData.LevelToLevelData.Values.Where(levelData => levelData.LevelStatus == LevelData.Status.Passed).ToList();

                return levelDatas.Count == 0 ? 0 : levelDatas.Max(data => data.Level);
            }
        }

        public int TotalLevelSurpassed
        {
            get
            {
                var levelDatas = this.uiTemplateUserLevelData.LevelToLevelData.Values.Where(levelData => levelData.LevelStatus != LevelData.Status.Locked).ToList();

                return levelDatas.Count == 0 ? 0 : levelDatas.Max(data => data.Level);
            }
        }


        public bool CheckLevelIsUnlockedStatus(int level)
        {
            var skippedLevel      = this.uiTemplateUserLevelData.LevelToLevelData.Values.LastOrDefault(levelData => levelData.LevelStatus == LevelData.Status.Skipped);
            var skippedLevelIndex = this.uiTemplateUserLevelData.LevelToLevelData.Values.ToList().IndexOf(skippedLevel);
            if (skippedLevelIndex == -1 && this.MaxLevel == 0 && level == 1)
            {
                return true;
            }

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

            for (int i = 0; i < rewardList.Count; i++)
            {
                if (this.uiTemplateInventoryDataController.GetItemData(rewardList[i]).CurrentStatus != UITemplateItemData.Status.Owned) return rewardList[i];
            }

            return null;
        }

        public Dictionary<string, int> GetAltReward(int currentLevel)
        {
            var levelUnlockReward = this.GetLevelUnlockReward(currentLevel);

            return levelUnlockReward < 0 ? null : this.uiTemplateLevelBlueprint.GetDataById(levelUnlockReward).AltReward;
        }

        private int GetLevelUnlockReward(int level)
        {
            for (int i = level; i <= this.uiTemplateLevelBlueprint.Count; i++)
            {
                if (this.uiTemplateLevelBlueprint.GetDataById(i).Rewards.Count > 0) return i;
            }

            return -1;
        }

        private int GetLastLevelUnlockReward(int level)
        {
            for (int i = level - 1; i > 0; i--)
            {
                if (this.uiTemplateLevelBlueprint.GetDataById(i).Rewards.Count > 0) return i;
            }

            return 0;
        }

    }
}