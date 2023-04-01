namespace TheOneStudio.UITemplate.UITemplate.Blueprints
{
    using BlueprintFlow.BlueprintReader;
    using UnityEngine;

    [BlueprintReader("UITemplateDecorCategory", true)]
    [CsvHeaderKey("Id")]
    public class UITemplateDecorCategoryBlueprint : GenericBlueprintReaderByRow<string, UITemplateDecorCategoryRecord>
    {
    }

    public class UITemplateDecorCategoryRecord
    {
        public string  Id                 { get; set; }
        public Vector3 OffsetPositionOnUI { get; set; }
        public Vector3 PositionOnScene    { get; set; }
        public Vector3 RotationOnScene    { get; set; }
    }
}