namespace TheOneStudio.UITemplate.UITemplate.Blueprints
{
    using BlueprintFlow.BlueprintReader;
    using UnityEngine.Scripting;

    [Preserve]
    [BlueprintReader("UITemplateItem", true)]
    public class UITemplateItemBlueprint : GenericBlueprintReaderByRow<string, UITemplateItemRecord>
    {
    }

    [Preserve]
    [CsvHeaderKey("Id")]
    public class UITemplateItemRecord
    {
        public string Id            { get; set; }
        public string Name          { get; set; }
        public string Description   { get; set; }
        public string ImageAddress  { get; set; }
        public string Category      { get; set; }
        public bool   IsDefaultItem { get; set; }
        public string AnimationName { get; set; }
        
    }
}