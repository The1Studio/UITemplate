namespace TheOneStudio.UITemplate.UITemplate.Blueprints
{
    using BlueprintFlow.BlueprintReader;
    using UnityEngine.Scripting;

    [Preserve]
    [BlueprintReader("UITemplateNotificationData", true)]
    public class UITemplateNotificationDataBlueprint : GenericBlueprintReaderByRow<string, UITemplateNotificationDataRecord>
    {
    }

    [Preserve]
    [CsvHeaderKey("Id")]
    public class UITemplateNotificationDataRecord
    {
        public string Id         { get; [Preserve] private set; }
        public string Title      { get; [Preserve] private set; }
        public string Body       { get; [Preserve] private set; }
        public bool   RandomAble { get; [Preserve] private set; }
    }
}