namespace TheOneStudio.UITemplate.UITemplate.Models.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using GameFoundation.Scripts.Utilities.Extension;
    using Sirenix.Serialization;
    using TheOneStudio.UITemplate.UITemplate.Blueprints;
    using TheOneStudio.UITemplate.UITemplate.Signals;
    using Zenject;

    public class UITemplateLevelDataController
    {
        #region inject

        private readonly UITemplateLevelBlueprint          uiTemplateLevelBlueprint;
        private readonly UITemplateUserLevelData           uiTemplateUserLevelData;
        private readonly UITemplateInventoryDataController uiTemplateInventoryDataController;
        private readonly SignalBus                         signalBus;

        #endregion

        public UITemplateLevelDataController(UITemplateLevelBlueprint uiTemplateLevelBlueprint, UITemplateUserLevelData uiTemplateUserLevelData, UITemplateInventoryDataController uiTemplateInventoryDataController, SignalBus signalBus)
        {
            this.uiTemplateLevelBlueprint          = uiTemplateLevelBlueprint;
            this.uiTemplateUserLevelData           = uiTemplateUserLevelData;
            this.uiTemplateInventoryDataController = uiTemplateInventoryDataController;
            this.signalBus                         = signalBus;
        }

        public List<LevelData> GetAllLevels() { return this.uiTemplateLevelBlueprint.Values.Select(levelRecord => this.GetLevelData(levelRecord.Level)).ToList(); }

        public LevelData GetLevelData(int level) { return this.uiTemplateUserLevelData.LevelToLevelData.GetOrAdd(level, () => new LevelData(level, LevelData.Status.Locked)); }

        public void ReplayLevel()
        {
            this.signalBus.Fire(new LevelStartedSignal( this.uiTemplateUserLevelData.CurrentLevel));
        }
        
        public void SelectLevel(int level)
        {
            this.uiTemplateUserLevelData.CurrentLevel = level;
            this.signalBus.Fire(new LevelStartedSignal(level));
        }

        public void GoToNextLevel()
        {
            this.signalBus.Fire(new LevelStartedSignal(this.uiTemplateUserLevelData.CurrentLevel));
        }

        public void PassCurrentLevel(int time = 0)
        {
            this.uiTemplateUserLevelData.SetLevelStatusByLevel(this.uiTemplateUserLevelData.CurrentLevel, LevelData.Status.Passed);
            this.signalBus.Fire(new LevelEndedSignal { Level = this.uiTemplateUserLevelData.CurrentLevel, IsWin = true, Time = time, CurrentIdToValue = null });
            this.uiTemplateUserLevelData.CurrentLevel++;
        }

        public void SkipCurrentLevel(int time = 0 )
        {
            this.uiTemplateUserLevelData.SetLevelStatusByLevel(this.uiTemplateUserLevelData.CurrentLevel, LevelData.Status.Skipped);

            this.signalBus.Fire(new LevelSkippedSignal { Level = this.uiTemplateUserLevelData.CurrentLevel, Time = time });

            this.GoToNextLevel();
        }

        public LevelData GetCurrentLevelData => this.GetLevelData(this.uiTemplateUserLevelData.CurrentLevel);

        public int MaxLevel
        {
            get
            {
                var levelDatas = this.uiTemplateUserLevelData.LevelToLevelData.Values.Where(levelData => levelData.LevelStatus == LevelData.Status.Passed).ToList();

                return levelDatas.Count == 0 ? 0 : levelDatas.Max(data => data.Level);
            }
        }

        public void LoseCurrentLevel (int time = 0)
        {
            this.signalBus.Fire(new LevelEndedSignal { Level = this.uiTemplateUserLevelData.CurrentLevel, IsWin = false, Time = time, CurrentIdToValue = null });
            this.GetLevelData(this.uiTemplateUserLevelData.CurrentLevel).LoseCount++;
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
            var temp = this.uiTemplateUserLevelData.LastUnlockRewardLevel;
            if(level == levelUnlockReward) this.UpdateLastUnlockRewardLevel(level);
            
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
            for (int i = level; i < this.uiTemplateLevelBlueprint.Count; i++)
            {
                if (this.uiTemplateLevelBlueprint.GetDataById(i).Rewards.Count > 0) return i;
            }

            return -1;
        }

        private void UpdateLastUnlockRewardLevel(int level)
        {
            this.uiTemplateUserLevelData.LastUnlockRewardLevel = level;
        }
    }
}