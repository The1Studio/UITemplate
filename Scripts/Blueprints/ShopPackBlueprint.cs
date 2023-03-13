namespace TheOneStudio.UITemplate.UITemplate.Blueprints
{
    using System.Collections.Generic;
    using BlueprintFlow.BlueprintReader;

    [CsvHeaderKey("Id")]
    [BlueprintReader("ShopPack", true)]
    public class ShopPackBlueprint : GenericBlueprintReaderByRow<string, ShopPackRecord>
    {
    }

    public class ShopPackRecord
    {
        public string    Id         { get; set; }
        public string    PackName   { get; set; }
        public List<int> PackValues { get; set; }

        public ProductType ProductType  { get; set; }
        public string      ImageAddress { get; set; }
        public string      Name         { get; set; }
        public string      Description  { get; set; }
    }

    public enum ProductType
    {
        Consumable,
        NonConsumable,
        Subscription,
        RemoveAds
    }
}