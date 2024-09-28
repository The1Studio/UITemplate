namespace TheOneStudio.UITemplate.UITemplate.Blueprints
{
    using System.Collections.Generic;
    using BlueprintFlow.BlueprintReader;
    using Newtonsoft.Json;
    using UnityEngine;
    using UnityEngine.Scripting;

    [Preserve]
    [BlueprintReader("UITemplateFTUE", true)]
    [CsvHeaderKey("Id")]
    public class UITemplateFTUEBlueprint : GenericBlueprintReaderByRow<string, UITemplateFTUERecord>
    {
    }

    [Preserve]
    public class UITemplateFTUERecord
    {
        public string       Id                     { get; [Preserve] private set; }
        public bool         EnableTrigger          { get; [Preserve] private set; }
        public string       NextStepId             { get; [Preserve] private set; }
        public List<string> PreviousSteps          { get; [Preserve] private set; }
        public string       ScreenLocation         { get; [Preserve] private set; }
        public List<string> RequireTriggerComplete { get; [Preserve] private set; }
        public string       RequireCondition       { get; [Preserve] private set; }
        public string       HighLightPath          { get; [Preserve] private set; }
        public bool         ButtonCanClick         { get; [Preserve] private set; }
        public float        Radius                 { get; [Preserve] private set; }
        public string       HandAnchor             { get; [Preserve] private set; }
        public Vector3      HandRotation           { get; [Preserve] private set; }
        public Vector2      HandSizeDelta          { get; [Preserve] private set; }

        public List<RequireCondition> GetRequireCondition() => JsonConvert.DeserializeObject<List<RequireCondition>>(this.RequireCondition);
    }

    [Preserve]
    public class RequireCondition
    {
        public string RequireId       { get; [Preserve] private set; }
        public string ConditionDetail { get; [Preserve] private set; }
    }
}