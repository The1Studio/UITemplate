namespace TheOneStudio.UITemplate.UITemplate.Blueprints
{
    using System.Collections.Generic;
    using BlueprintFlow.BlueprintReader;

    [BlueprintReader("UITemplateGachaJackpot", true)]
    public class UITemplateGachaJackpotBlueprint : GenericBlueprintReaderByRow<string, UITemplateGachaJackpotRecord>
    {
    }

    [CsvHeaderKey("Id")]
    public class UITemplateGachaJackpotRecord
    {
        public string                  Id     { get; set; }
        public string                  Icon   { get; set; }
        public Dictionary<string, int> Reward { get; set; }
        public float                   Weight { get; set; }
    }
}