namespace TheOneStudio.UITemplate.UITemplate.Blueprints
{
    using BlueprintFlow.BlueprintReader;
    using TheOneStudio.UITemplate.UITemplate.Models;

    [BlueprintReader("UITemplateItem", true)]
    public class UITemplateItemBlueprint : GenericBlueprintReaderByRow<string, UITemplateItemRecord>
    {
    }

    [CsvHeaderKey("Id")]
    public class UITemplateItemRecord
    {
        public string                        Id           { get; set; }
        public string                        Name         { get; set; }
        public string                        Description  { get; set; }
        public string                        ImageAddress { get; set; }
        public UITemplateItemData.UnlockType UnlockType   { get; set; }
        public string                        CurrencyID    { get; set; }
        public int                           Price        { get; set; }
        public string                        Category     { get; set; }
    }
}