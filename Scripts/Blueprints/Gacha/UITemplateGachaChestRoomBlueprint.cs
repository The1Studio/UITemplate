namespace TheOneStudio.UITemplate.UITemplate.Blueprints.Gacha
{
    using System.Collections.Generic;
    using BlueprintFlow.BlueprintReader;

    [CsvHeaderKey("Id")]
    [BlueprintReader("UITemplateGachaChestRoom", true)]
    public class UITemplateGachaChestRoomBlueprint : GenericBlueprintReaderByRow<string, UITemplateGachaChestRoomRecord>
    {
        
    }
    
    public class UITemplateGachaChestRoomRecord
    {
        public string                  Id          { get; set; }
        public string                  Icon        { get; set; }
        public Dictionary<string, int> Reward      { get; set; }
        public float                   Weight      { get; set; }
        public bool                    IsBestPrize { get; set; }
    }
}