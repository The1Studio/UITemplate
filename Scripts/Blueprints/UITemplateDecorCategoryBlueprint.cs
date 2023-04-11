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
        public string         Id              { get; set; }
        public Vector3        ButtonPosition  { get; set; }
        public Vector3        PositionOnScene { get; set; }
        public Vector3        RotationOnScene { get; set; }
        public DecorationMode Mode            { get; set; }
        public bool           IsScaleRoot     { get; set; }
        public int            Layer           { get; set; }
    }

    public enum DecorationMode
    {
        Theme2D,
        Theme3D
    }
}