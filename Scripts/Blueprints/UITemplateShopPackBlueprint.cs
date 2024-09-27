namespace TheOneStudio.UITemplate.UITemplate.Blueprints
{
    using System.Collections.Generic;
    using System.Linq;
    using BlueprintFlow.BlueprintReader;
    using ServiceImplementation.IAPServices;
    using UnityEngine.Scripting;

    [Preserve]
    [BlueprintReader("UITemplateShopPack", true)]
    public class UITemplateShopPackBlueprint : GenericBlueprintReaderByRow<string, ShopPackRecord>
    {
        public List<ShopPackRecord> GetPack()
        {
            #if UNITY_ANDROID
            return this.Values.Where(x => x.Platforms.Contains("Android")).ToList();
            #elif UNITY_IOS || UNITY_IPHONE
            return this.Values.Where(x => x.Platforms.Contains("IOS")).ToList();
            #else
            return this.Values.ToList();
            #endif
        }
    }

    [Preserve]
    [CsvHeaderKey("Id")]
    public class ShopPackRecord
    {
        public string                                                Id                    { get; [Preserve] private set; }
        public List<string>                                          Platforms             { get; [Preserve] private set; }
        public ProductType                                           ProductType           { get; [Preserve] private set; }
        public string                                                ImageAddress          { get; [Preserve] private set; }
        public string                                                Name                  { get; [Preserve] private set; }
        public string                                                Description           { get; [Preserve] private set; }
        public string                                                DefaultPrice          { get; [Preserve] private set; }
        public BlueprintByRow<string, UITemplateRewardBlueprintData> RewardIdToRewardDatas { get; [Preserve] private set; }
    }

    [Preserve]
    [CsvHeaderKey("RewardId")]
    public class UITemplateRewardBlueprintData
    {
        public string RewardId              { get; [Preserve] private set; }
        public int    RewardValue           { get; [Preserve] private set; }
        public int    Repeat                { get; [Preserve] private set; }
        public string RewardIcon            { get; [Preserve] private set; }
        public string RewardContent         { get; [Preserve] private set; }
        public string AddressableFlyingItem { get; [Preserve] private set; }
    }
}