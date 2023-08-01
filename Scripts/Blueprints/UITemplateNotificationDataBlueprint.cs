namespace TheOneStudio.UITemplate.UITemplate.Blueprints
{
    using BlueprintFlow.BlueprintReader;

    [BlueprintReader("UITemplateNotificationData", true)]
    [CsvHeaderKey("Id")]
    public class UITemplateNotificationDataBlueprint : GenericBlueprintReaderByRow<string, UITemplateNotificationDataRecord>
    {
    }

    public class UITemplateNotificationDataRecord
    {
        public string Id         { get; set; }
        public string Title      { get; set; }
        public string Body       { get; set; }
        public bool   RandomAble { get; set; }
    }
}