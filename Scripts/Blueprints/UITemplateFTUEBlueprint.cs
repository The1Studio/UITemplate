namespace TheOneStudio.UITemplate.UITemplate.Blueprints
{
    using System.Collections.Generic;
    using BlueprintFlow.BlueprintReader;
    using GameFoundation.Scripts.Utilities.Extension;
    using Newtonsoft.Json;
    using UnityEngine;

    [BlueprintReader("UITemplateFTUE", true)]
    [CsvHeaderKey("Id")]
    public class UITemplateFTUEBlueprint : GenericBlueprintReaderByRow<string, UITemplateFTUERecord>
    {
    }

    public class UITemplateFTUERecord
    {
        public string       Id                     { get; set; }
        public bool         EnableTrigger          { get; set; }
        public string       NextStepId             { get; set; }
        public string       ScreenLocation         { get; set; }
        public List<string> RequireTriggerComplete { get; set; } = new();
        public string       RequireCondition       { get; set; }
        public string       HighLightPath          { get; set; }
        public bool         ButtonCanClick         { get; set; }
        public float        Radius                 { get; set; }
        public string       HandAnchor             { get; set; }
        public Vector3      HandRotation           { get; set; }
        public Vector2      HandSizeDelta          { get; set; }

        public List<RequireCondition> GetRequireCondition() => JsonConvert.DeserializeObject<List<RequireCondition>>(this.RequireCondition);

        /// <summary>
        /// For test Generate Json require condition
        /// </summary>
        public UITemplateFTUERecord()
        {
            var list = new List<RequireCondition>();
            list.Add(new RequireCondition() { RequireId = "Round_level", RequireValue = "8" });
#if UNITY_EDITOR
            Debug.Log($"{list.ToJson()}");
#endif
        }
    }

    public class RequireCondition
    {
        public string RequireId    { get; set; }
        public string Condition    { get; set; } = "Equal"; //Equal, NotEqual, Higher, Lower, Higher, Lower, HigherEqual, LowerEqual
        public string RequireValue { get; set; }
    }
}