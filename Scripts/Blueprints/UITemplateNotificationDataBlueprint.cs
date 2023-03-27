namespace TheOneStudio.UITemplate.UITemplate.Blueprints
{
    using BlueprintFlow.BlueprintReader;

    [BlueprintReader("UITemplateNotificationData", true)]
    [CsvHeaderKey("Id")]
    public class UITemplateNotificationDataBlueprint : GenericBlueprintReaderByRow<int, UITemplateNotificationDataRecord>
    {
    }

    public class UITemplateNotificationDataRecord
    {
        public string Id    { get; set; }
        public string Title { get; set; }
        public string Body  { get; set; }

        public string GetTitle(object[] data) => string.Format(this.Title, data);
        public string GetBody(object[] data)  => string.Format(this.Body, data);
    }
}