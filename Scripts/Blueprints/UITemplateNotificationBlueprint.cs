namespace TheOneStudio.UITemplate.UITemplate.Blueprints
{
    using System.Collections.Generic;
    using BlueprintFlow.BlueprintReader;
    using UnityEngine.Scripting;

    [Preserve]
    [BlueprintReader("UITemplateNotification", true)]
    public class UITemplateNotificationBlueprint : GenericBlueprintReaderByRow<string, UITemplateNotificationRecord>
    {
    }

    [Preserve]
    [CsvHeaderKey("Id")]
    public class UITemplateNotificationRecord
    {
        public string    Id            { get; [Preserve] private set; }
        public List<int> HourRangeShow { get; [Preserve] private set; }
        public List<int> TimeToShow    { get; [Preserve] private set; }
        public bool      RandomAble    { get; [Preserve] private set; }
        public string    Title         { get; [Preserve] private set; }
        public string    Body          { get; [Preserve] private set; }
    }
}