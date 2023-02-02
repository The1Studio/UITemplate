namespace UITemplate.Scripts.Blueprints
{
    using BlueprintFlow.BlueprintReader;

    [BlueprintReader("UITemplateFTUE", true)]
    [CsvHeaderKey("Id")]
    public class UITemplateFTUEBlueprint : GenericBlueprintReaderByRow<string, UITemplateFTUERecord>
    {
        
    }

    public class UITemplateFTUERecord
    {
        public string Id;
        public string StartCondition;
        public string FinishCondition;
    }
}