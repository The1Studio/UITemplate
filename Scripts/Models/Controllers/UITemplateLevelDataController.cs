namespace TheOneStudio.UITemplate.UITemplate.Models.Controllers
{
    using System.Collections.Generic;
    using System.Linq;
    using GameFoundation.Scripts.Utilities.Extension;
    using TheOneStudio.UITemplate.UITemplate.Blueprints;
    using TheOneStudio.UITemplate.UITemplate.Signals;
    using Zenject;

    public class UITemplateLevelDataController
    {
        #region inject

        private readonly UITemplateLevelBlueprint uiTemplateLevelBlueprint;
        private readonly UITemplateUserLevelData  uiTemplateUserLevelData;
        private readonly SignalBus                signalBus;

        #endregion

        public UITemplateLevelDataController(UITemplateLevelBlueprint uiTemplateLevelBlueprint, UITemplateUserLevelData uiTemplateUserLevelData,
                                             SignalBus signalBus)
        {
            this.uiTemplateLevelBlueprint = uiTemplateLevelBlueprint;
            this.uiTemplateUserLevelData  = uiTemplateUserLevelData;
            this.signalBus                = signalBus;
        }

        public List<LevelData> GetAllLevels() { return this.uiTemplateLevelBlueprint.Values.Select(levelRecord => this.GetLevelData(levelRecord.Level)).ToList(); }

        public LevelData GetLevelData(int level) { return this.uiTemplateUserLevelData.LevelToLevelData.GetOrAdd(level, () => new LevelData(level, LevelData.Status.Locked)); }

        public void SelectLevel(int level)
        {
            this.uiTemplateUserLevelData.CurrentLevel = level;
            this.signalBus.Fire(new LevelStartedSignal(level));
        }

        public void GoToNextLevel()
        {
            this.uiTemplateUserLevelData.CurrentLevel++;
            this.signalBus.Fire(new LevelStartedSignal(this.uiTemplateUserLevelData.CurrentLevel));
        }

        public void PassCurrentLevel()
        {
            this.uiTemplateUserLevelData.SetLevelStatusByLevel(this.uiTemplateUserLevelData.CurrentLevel, LevelData.Status.Passed);
            this.GoToNextLevel();
        }

        public void SkipCurrentLevel()
        {
            this.uiTemplateUserLevelData.SetLevelStatusByLevel(this.uiTemplateUserLevelData.CurrentLevel, LevelData.Status.Skipped);

            this.signalBus.Fire(new LevelSkippedSignal
            {
                Level = this.uiTemplateUserLevelData.CurrentLevel,
                Time  = 0
            });

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
    }
}