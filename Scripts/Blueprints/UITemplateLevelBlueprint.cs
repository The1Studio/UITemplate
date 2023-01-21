namespace UITemplate.Scripts.Blueprints
{
    using BlueprintFlow.BlueprintReader;

    [BlueprintReader("UITemplateLevel", true)]
    [CsvHeaderKey("Level")]
    public class UITemplateLevelBlueprint : GenericBlueprintReaderByRow<int, UITemplateLevelRecord>
    {
        
    }

    public class UITemplateLevelRecord
    {
        public int    Level;
    }
}