namespace TheOneStudio.UITemplate.UITemplate.Blueprints
{
    using BlueprintFlow.BlueprintReader;
    using UnityEngine.Scripting;

    [Preserve]
    [BlueprintReader("UITemplateCategoryItem", true)]
    public class UITemplateCategoryItemBlueprint : GenericBlueprintReaderByRow<string, UITemplateCategoryItemRecord>
    {
    }

    [Preserve]
    [CsvHeaderKey("Id")]
    public class UITemplateCategoryItemRecord
    {
        public string Id    { get; [Preserve] private set; }
        public string Icon  { get; [Preserve] private set; }
        public string Title { get; [Preserve] private set; }
    }
}