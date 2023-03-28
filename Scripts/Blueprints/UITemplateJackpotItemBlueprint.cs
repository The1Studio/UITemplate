namespace TheOneStudio.UITemplate.UITemplate.Blueprints
{
    using System.Collections.Generic;
    using BlueprintFlow.BlueprintReader;

    [BlueprintReader("UITemplateJackpotItem", true)]
    public class UITemplateJackpotItemBlueprint : GenericBlueprintReaderByRow<string, UITemplateJackpotItemRecord>
    {
    }

    [CsvHeaderKey("Id")]
    public class UITemplateJackpotItemRecord
    {
        public string                        Id     { get; set; }
        public string                        Icon   { get; set; }
        public List<Dictionary<string, int>> Reward { get; set; }
        public string                        Weight { get; set; }
    }
}