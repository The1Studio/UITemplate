namespace TheOneStudio.UITemplate.UITemplate.Blueprints
{
    using BlueprintFlow.BlueprintReader;
    using TheOneStudio.UITemplate.UITemplate.Models;
    using UnityEngine.Scripting;

    [Preserve]
    [BlueprintReader("UITemplateShop", true)]
    [CsvHeaderKey("Id")]
    public class UITemplateShopBlueprint : GenericBlueprintReaderByRow<string, UITemplateShopRecord>
    {
    }

    [Preserve]
    public class UITemplateShopRecord
    {
        public string                        Id;
        public UITemplateItemData.UnlockType UnlockType;
        public string                        CurrencyID;
        public int                           Price;
    }
}