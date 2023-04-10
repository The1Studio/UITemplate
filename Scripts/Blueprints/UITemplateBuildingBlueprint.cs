namespace TheOneStudio.UITemplate.UITemplate.Blueprints
{
    using System.Collections.Generic;
    using BlueprintFlow.BlueprintReader;

    [BlueprintReader("UITemplateBuilding", true)]
    [CsvHeaderKey("Id")]
    public class UITemplateBuildingBlueprint : GenericBlueprintReaderByRow<string, UITemplateBuildingRecord>
    {
    }

    public class UITemplateBuildingRecord
    {
        public string                    Id            { get; set; }
        public string                    UnlockPrice   { get; set; }
        public Dictionary<string, float> EarnPerSecond { get; set; }
    }
}