namespace TheOneStudio.UITemplate.UITemplate.Blueprints
{
    using System.Collections.Generic;
    using BlueprintFlow.BlueprintReader;

    [BlueprintReader("UITemplateNotification", true)]
    [CsvHeaderKey("Id")]
    public class UITemplateNotificationBlueprint : GenericBlueprintReaderByRow<int, UITemplateNotificationRecord>
    {
    }

    public class UITemplateNotificationRecord
    {
        public string    Id            { get; set; }
        public List<int> HourRangeShow { get; set; } = new();
        public List<int> TimeToShow    { get; set; } = new();
        public string    Repeat        { get; set; } = "None";
        public int       Title         { get; set; } = new();
        public int       Body          { get; set; } = new();
    }
}