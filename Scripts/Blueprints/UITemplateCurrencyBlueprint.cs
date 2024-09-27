namespace TheOneStudio.UITemplate.UITemplate.Blueprints
{
    using BlueprintFlow.BlueprintReader;
    using UnityEngine.Scripting;

    [Preserve]
    [BlueprintReader("UITemplateCurrency", true)]
    public class UITemplateCurrencyBlueprint : GenericBlueprintReaderByRow<string, UITemplateCurrencyRecord>
    {
    }

    [Preserve]
    [CsvHeaderKey("Id")]
    public class UITemplateCurrencyRecord
    {
        public string Id              { get; [Preserve] private set; }
        public string Name            { get; [Preserve] private set; }
        public string IconAddressable { get; [Preserve] private set; }
        public int    Max             { get; [Preserve] private set; }
        public string FlyingObject    { get; [Preserve] private set; }
    }
}