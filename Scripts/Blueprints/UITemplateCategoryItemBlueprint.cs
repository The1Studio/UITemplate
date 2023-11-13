namespace TheOneStudio.UITemplate.UITemplate.Blueprints
{
    using BlueprintFlow.BlueprintReader;

   
    [BlueprintReader("UITemplateCategoryItem", true)]
    public class UITemplateCategoryItemBlueprint : GenericBlueprintReaderByRow<string, UITemplateCategoryItemRecord>
    {
    }

    [CsvHeaderKey("Id")]
    public class UITemplateCategoryItemRecord
    {
        public string Id    { get; set; }
        public string Icon  { get; set; }
        public string Title { get; set; }
    }
}