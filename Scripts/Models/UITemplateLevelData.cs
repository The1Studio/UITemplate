namespace TheOneStudio.UITemplate.UITemplate.Models
{
    using System.Collections.Generic;
    using System.Linq;
    using GameFoundation.Scripts.Interfaces;
    using GameFoundation.Scripts.Utilities.Extension;
    using Newtonsoft.Json;
    using TheOneStudio.UITemplate.UITemplate.Blueprints;
    using TheOneStudio.UITemplate.UITemplate.Signals;
    using Zenject;

    public class UITemplateUserLevelData : ILocalData
    {
        #region inject

        private readonly UITemplateLevelBlueprint uiTemplateLevelBlueprint;
        private readonly SignalBus                signalBus;

        #endregion

        public int                        CurrentLevel { get; private set; } = 1;
        public Dictionary<int, LevelData> LevelToLevelData = new();

        public UITemplateUserLevelData(UITemplateLevelBlueprint uiTemplateLevelBlueprint, SignalBus signalBus)
        {
            this.uiTemplateLevelBlueprint = uiTemplateLevelBlueprint;
            this.signalBus                = signalBus;
        }

        public List<LevelData> GetAllLevels() { return this.uiTemplateLevelBlueprint.Values.Select(levelRecord => this.GetLevelData(levelRecord.Level)).ToList(); }

        public LevelData GetLevelData(int level)
        {
            return this.LevelToLevelData.GetOrAdd(level, () => new LevelData(level, LevelData.Status.Locked));
        }

        public void SelectLevel(int level)
        {
            this.CurrentLevel = level;
            this.signalBus.Fire(new LevelStartedSignal(level));
        }

        public void GoToNextLevel()
        {
            this.CurrentLevel++;
            this.signalBus.Fire(new LevelStartedSignal(this.CurrentLevel));
        }

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
            this.StarCount      = starCount;
        }

        public enum Status
        {
            Locked,
            Passed,
            Skipped,
        }
    }
}