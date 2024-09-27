namespace TheOneStudio.UITemplate.UITemplate.Blueprints
{
    using System.Collections.Generic;
    using BlueprintFlow.BlueprintReader;
    using UnityEngine.Scripting;

    [Preserve]
    [BlueprintReader("UITemplateLevel", true)]
    [CsvHeaderKey("Level")]
    public class UITemplateLevelBlueprint : GenericBlueprintReaderByRow<int, UITemplateLevelRecord>
    {
    }

    [Preserve]
    public class UITemplateLevelRecord
    {
        public int    Level       { get; set; }
        public string PrefabName  { get; set; }
        public string Description { get; set; }

        // List of item ids that can be rewarded (for item that cannot be have multiple instances)
        public List<string> Rewards { get; set; }

        // Dictionary of item ids and their quantity that can be rewarded (for item that can have multiple instances)
        public Dictionary<string, int> AltReward { get; set; }
    }
}