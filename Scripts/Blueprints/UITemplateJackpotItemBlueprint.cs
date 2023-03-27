namespace TheOneStudio.UITemplate.UITemplate.Blueprints
{
    using BlueprintFlow.BlueprintReader;

    [BlueprintReader("UITemplateJackpotItem", true)]
    public class UITemplateJackpotItemBlueprint: GenericBlueprintReaderByRow<string, UITemplateCategoryItemRecord>
    {
        
    }
    
    [CsvHeaderKey("Id")]
    public class UITemplateJackpotItemRecord
    {
        public string Id   { get; set; }
        public string Icon { get; set; }
    }
}