namespace TheOneStudio.UITemplate.UITemplate.Blueprints
{
    using BlueprintFlow.BlueprintReader;
    using TheOneStudio.UITemplate.UITemplate.Models;

    [BlueprintReader("UITemplateShop", true)]
    [CsvHeaderKey("Id")]
    public class UITemplateShopBlueprint : GenericBlueprintReaderByRow<string, UITemplateShopRecord>
    {
    }

    public class UITemplateShopRecord
    {
        public string                        Id;
        public UITemplateItemData.UnlockType UnlockType;
        public string                        CurrencyID;
        public int                           Price;
        public string                        IapPackId;
    }
}