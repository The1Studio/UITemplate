namespace TheOneStudio.UITemplate.UITemplate.Blueprints
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using BlueprintFlow.BlueprintReader;
    using ServiceImplementation.IAPServices;
    using UnityEngine.Scripting;

    [Preserve]
    [CsvHeaderKey("Id")]
    [BlueprintReader("UITemplateShopPack", true)]
    public class UITemplateShopPackBlueprint : GenericBlueprintReaderByRow<string, ShopPackRecord>
    {
        public List<ShopPackRecord> GetPack()
        {
#if UNITY_ANDROID
            return this.Values.Where(x => x.Platforms.Contains("Android")).ToList();
#elif UNITY_IOS||UNITY_IPHONE
            return this.Values.Where(x => x.Platforms.Contains("IOS")).ToList();
#else
            return this.Values.ToList();
#endif
        }
    }

    public class ShopPackRecord
    {
        public string                                                Id                    { get; set; }
        public List<string>                                          Platforms             { get; set; }
        public ProductType                                           ProductType           { get; set; }
        public string                                                ImageAddress          { get; set; }
        public string                                                Name                  { get; set; }
        public string                                                Description           { get; set; }
        public string                                                DefaultPrice          { get; set; }
        public BlueprintByRow<string, UITemplateRewardBlueprintData> RewardIdToRewardDatas { get; set; }
    }

    [Serializable]
    [CsvHeaderKey("RewardId")]
    public class UITemplateRewardBlueprintData
    {
        public string RewardId              { get; set; }
        public int    RewardValue           { get; set; }
        public int    Repeat                { get; set; }
        public string RewardIcon            { get; set; }
        public string RewardContent         { get; set; }
        public string AddressableFlyingItem { get; set; }
    }
}