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
        public string              Id;
        public string              Name;
        public string              Description;
        public string              Category;
        public UITemplateItemData.UnlockType UnlockType;
        public float               Price;
    }
}