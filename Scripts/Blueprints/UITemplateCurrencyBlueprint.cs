namespace TheOneStudio.UITemplate.UITemplate.Blueprints
{
    using BlueprintFlow.BlueprintReader;

    [BlueprintReader("UITemplateCurrency", true)]
    [CsvHeaderKey("Id")]
    public class UITemplateCurrencyBlueprint : GenericBlueprintReaderByRow<string, UITemplateCurrencyRecord>
    {
    }

    public class UITemplateCurrencyRecord
    {
        public string Id;
        public int    Max;
    }
}