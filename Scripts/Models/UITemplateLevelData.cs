namespace UITemplate.Scripts.Models
{
    using System.Collections.Generic;
    using System.Linq;
    using GameFoundation.Scripts.Utilities.Extension;
    using Newtonsoft.Json;
    using UITemplate.Scripts.Blueprints;

    public class UITemplateLevelData
    {
        #region inject

        private readonly UITemplateLevelBlueprint uiTemplateLevelBlueprint;

        #endregion

        public int                        CurrentLevel;
        public Dictionary<int, LevelData> LevelToLevelData = new();

        public UITemplateLevelData(UITemplateLevelBlueprint uiTemplateLevelBlueprint) { this.uiTemplateLevelBlueprint = uiTemplateLevelBlueprint; }

        public List<LevelData> GetAllLevels() { return this.uiTemplateLevelBlueprint.Values.Select(levelRecord => this.GetLevelData(levelRecord.Level)).ToList(); }

        public LevelData GetLevelData(int level)
        {
            return this.LevelToLevelData.GetOrAdd(level, () =>
            {
                var record = this.uiTemplateLevelBlueprint.GetDataById(level);
                return new LevelData(record);
            });
        }
    }

    public class LevelData
    {
        public int    Level;
        public Status LevelStatus;

        [JsonIgnore] public UITemplateLevelRecord Record;

        public LevelData(UITemplateLevelRecord record) { this.Record = record; }
        
        

        public enum Status
        {
            Locked,
            Passed,
            Skipped,
        }
    }
}