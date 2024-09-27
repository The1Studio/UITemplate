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
        public string Id    { get; set; }
        public string Icon  { get; set; }
        public string Title { get; set; }
    }
}