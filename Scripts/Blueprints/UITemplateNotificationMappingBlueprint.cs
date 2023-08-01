namespace TheOneStudio.UITemplate.UITemplate.Blueprints
{
    using BlueprintFlow.BlueprintReader;

    [BlueprintReader("UITemplateNotificationMapping", true)]
    [CsvHeaderKey("Id")]
    public class
        UITemplateNotificationMappingBlueprint : GenericBlueprintReaderByRow<string,
            UITemplateNotificationMappingRecord>
    {
    }

    public class UITemplateNotificationMappingRecord
    {
        public string Id          { get; set; }
        public string Replacement { get; set; }
    }
}