namespace TheOneStudio.UITemplate.UITemplate.Blueprints.Gacha
{
    using System.Collections.Generic;
    using BlueprintFlow.BlueprintReader;

    [BlueprintReader("UITemplateGachaLuckySpin", true)]
    public class UITemplateLuckySpinBlueprint : GenericBlueprintReaderByRow<string, UITemplateLuckySpinRecord>
    {
    }

    public class UITemplateLuckySpinRecord
    {
        public string                  Id      { get; set; }
        public string                  Icon    { get; set; }
        public Dictionary<string, int> Rewards { get; set; }
        public int                     Weight  { get; set; }
    }
}