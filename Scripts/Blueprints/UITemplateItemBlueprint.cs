namespace TheOneStudio.UITemplate.UITemplate.Blueprints
{
    using BlueprintFlow.BlueprintReader;
    using UnityEngine.Scripting;

    [Preserve]
    [BlueprintReader("UITemplateItem", true)]
    public class UITemplateItemBlueprint : GenericBlueprintReaderByRow<string, UITemplateItemRecord>
    {
    }

    [Preserve]
    [CsvHeaderKey("Id")]
    public class UITemplateItemRecord
    {
        public string                        Id            { get; [Preserve] private set; }
        public string                        Name          { get; [Preserve] private set; }
        public string                        Description   { get; [Preserve] private set; }
        public string                        ImageAddress  { get; [Preserve] private set; }
        public string                        Category      { get; [Preserve] private set; }
        public bool                          IsDefaultItem { get; [Preserve] private set; }
    }
}