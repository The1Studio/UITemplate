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
        public string                                                Id                 { get; set; }
        public BlueprintByRow<string, NotificationContentDataRecord> ContentDataRecords { get; set; }
    }

    [CsvHeaderKey("ContentId")]
    public class NotificationContentDataRecord
    {
        public string ContentId { get; set; }
        public string Title     { get; set; }
        public string Body      { get; set; }
    }
}