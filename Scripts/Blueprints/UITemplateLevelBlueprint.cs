namespace TheOneStudio.UITemplate.UITemplate.Blueprints
{
    using System.Collections.Generic;
    using BlueprintFlow.BlueprintReader;
    using UnityEngine.Scripting;

    [Preserve]
    [BlueprintReader("UITemplateLevel", true)]
    public class UITemplateLevelBlueprint : GenericBlueprintReaderByRow<int, UITemplateLevelRecord>
    {
    }

    [Preserve]
    [CsvHeaderKey("Level")]
    public class UITemplateLevelRecord
    {
        public int    Level       { get; [Preserve] private set; }
        public string PrefabName  { get; [Preserve] private set; }
        public string Description { get; [Preserve] private set; }

        // List of item ids that can be rewarded (for item that cannot be have multiple instances)
        public List<string> Rewards { get; [Preserve] private set; }

        // Dictionary of item ids and their quantity that can be rewarded (for item that can have multiple instances)
        public Dictionary<string, int> AltReward { get; [Preserve] private set; }
    }
}