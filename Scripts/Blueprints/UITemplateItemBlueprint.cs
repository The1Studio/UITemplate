namespace UITemplate.Scripts.Blueprints
{
    using BlueprintFlow.BlueprintReader;
    using UITemplate.Scripts.Models;

    [CsvHeaderKey("Id")]
    [BlueprintReader("UITemplateItem", true)]
    public class UITemplateItemBlueprint : GenericBlueprintReaderByRow<string, UITemplateItemRecord>
    {
        
    }

    public class UITemplateItemRecord
    {
        public string              Id          { get; set; }
        public string              Name        { get; set; }
        public string              Description { get; set; }
        public ItemData.UnlockType UnlockType  { get; set; }
        public float               Price       { get; set; }
    }
}