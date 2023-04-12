namespace TheOneStudio.UITemplate.UITemplate.Blueprints
{
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
        public string       Id          { get; set; }
        public List<string> Platforms   { get; set; }
        public string       PackName    { get; set; }
        public bool         IsRemoveAds { get; set; }
        public List<int>    PackValues  { get; set; }

        public ProductType ProductType  { get; set; }
        public string      ImageAddress { get; set; }
        public string      Name         { get; set; }
        public string      Description  { get; set; }
    }
}