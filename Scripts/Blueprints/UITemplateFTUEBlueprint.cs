namespace TheOneStudio.UITemplate.UITemplate.Blueprints
{
    using System.Collections.Generic;
    using BlueprintFlow.BlueprintReader;
    using UnityEngine;

    [BlueprintReader("UITemplateFTUE", true)]
    [CsvHeaderKey("Id")]
    public class UITemplateFTUEBlueprint : GenericBlueprintReaderByRow<string, UITemplateFTUERecord>
    {
    }

    public class UITemplateFTUERecord
    {
        public string       Id               { get; set; }
        public bool         EnableTrigger    { get; set; } 
        public string       ScreenLocation   { get; set; } 
        public List<string> RequireCondition { get; set; } = new();
        public string       HighLightPath    { get; set; } 
        public bool         ButtonCanClick   { get; set; } 
        public float        Radius           { get; set; } 
        public string       HandAnchor       { get; set; } 
        public Vector3      HandRotation     { get; set; } 
        public Vector2      HandSizeDelta    { get; set; } 
    }
}