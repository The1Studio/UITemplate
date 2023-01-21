namespace UITemplate.Scripts.Blueprints
{
    using BlueprintFlow.BlueprintReader;
    using UITemplate.Scripts.Models;

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
        public ItemData.UnlockType UnlockType;
        public float               Price;
    }
}