namespace TheOneStudio.UITemplate.UITemplate.Blueprints
{
    using BlueprintFlow.BlueprintReader;
    using TheOneStudio.UITemplate.UITemplate.Models;
    using UnityEngine.Scripting;

    [Preserve]
    [BlueprintReader("UITemplateShop", true)]
    public class UITemplateShopBlueprint : GenericBlueprintReaderByRow<string, UITemplateShopRecord>
    {
    }

    [Preserve]
    [CsvHeaderKey("Id")]
    public class UITemplateShopRecord
    {
        public string                        Id         { get; [Preserve] private set; }
        public UITemplateItemData.UnlockType UnlockType { get; [Preserve] private set; }
        public string                        CurrencyID { get; [Preserve] private set; }
        public int                           Price      { get; [Preserve] private set; }
    }
}