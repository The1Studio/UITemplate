namespace TheOneStudio.UITemplate.UITemplate.Blueprints
{
    using System;
    using System.Collections.Generic;
    using BlueprintFlow.BlueprintReader;
    using ServiceImplementation.IAPServices;

    [CsvHeaderKey("Id")]
    [BlueprintReader("UITemplateShopPack", true)]
    public class UITemplateShopPackBlueprint : GenericBlueprintReaderByRow<string, ShopPackRecord>
    {
    }

    public class ShopPackRecord
    {
        public string                                                Id                    { get; set; }
        public List<string>                                          Platforms             { get; set; }
        public ProductType                                           ProductType           { get; set; }
        public string                                                ImageAddress          { get; set; }
        public string                                                Name                  { get; set; }
        public string                                                Description           { get; set; }
        public BlueprintByRow<string, UITemplateRewardBlueprintData> RewardIdToRewardDatas { get; set; }
    }

    [Serializable]
    [CsvHeaderKey("RewardId")]
    public class UITemplateRewardBlueprintData
    {
        public string RewardId              { get; set; }
        public string RewardValue           { get; set; }
        public int    Repeat                { get; set; }
        public string RewardIcon            { get; set; }
        public string AddressableFlyingItem { get; set; }
    }
}