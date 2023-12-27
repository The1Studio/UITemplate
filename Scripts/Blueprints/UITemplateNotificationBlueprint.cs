namespace TheOneStudio.UITemplate.UITemplate.Blueprints
{
    using System.Collections.Generic;
    using BlueprintFlow.BlueprintReader;

    [BlueprintReader("UITemplateNotification", true)]
    [CsvHeaderKey("Id")]
    public class UITemplateNotificationBlueprint : GenericBlueprintReaderByRow<string, UITemplateNotificationRecord>
    {
    }

    public class UITemplateNotificationRecord
    {
        public string    Id             { get; set; }
        public string    DataId         { get; set; }
        public List<int> HourRangeShow  { get; set; } = new();
        public List<int> TimeToShow     { get; set; } = new();
        public bool      ShowOneContent { get; set; }
        public bool      AutoShow       { get; set; }
    }
}