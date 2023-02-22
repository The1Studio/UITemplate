namespace TheOneStudio.UITemplate.UITemplate.Models
{
    using System.Collections.Generic;
    using System.Linq;
    using GameFoundation.Scripts.Interfaces;
    using GameFoundation.Scripts.Utilities.Extension;
    using Newtonsoft.Json;
    using TheOneStudio.UITemplate.UITemplate.Blueprints;

    public class UITemplateUserLevelData:ILocalData
    {
        #region inject

        private readonly UITemplateLevelBlueprint uiTemplateLevelBlueprint;

        #endregion

        public int                        CurrentLevel = 1;
        public Dictionary<int, LevelData> LevelToLevelData = new();

        public UITemplateUserLevelData(UITemplateLevelBlueprint uiTemplateLevelBlueprint) { this.uiTemplateLevelBlueprint = uiTemplateLevelBlueprint; }

        public List<LevelData> GetAllLevels() { return this.uiTemplateLevelBlueprint.Values.Select(levelRecord => this.GetLevelData(levelRecord.Level)).ToList(); }

        public LevelData GetLevelData(int level)
        {
            return this.LevelToLevelData.GetOrAdd(level, () =>
            {
                var record = this.uiTemplateLevelBlueprint.GetDataById(level);
                return new LevelData(record, level, LevelData.Status.Locked);
            });
        }

        public void Init() {  }
    }

    public class LevelData
    {
        public int    Level;
        public Status LevelStatus;
        public int    StarCount;

        [JsonIgnore] public UITemplateLevelRecord Record;
        
        public LevelData(UITemplateLevelRecord record,int level, Status levelStatus, int starCount = 0)
        {
            Record = record;
            Level = level;
            LevelStatus = levelStatus;
            StarCount = starCount;
        }

        public enum Status
        {
            Locked,
            Passed,
            Skipped,
        }
    }
}