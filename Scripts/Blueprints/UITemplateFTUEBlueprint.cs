namespace TheOneStudio.UITemplate.UITemplate.Blueprints
{
    using System.Collections.Generic;
    using BlueprintFlow.BlueprintReader;
    using Newtonsoft.Json;
    using Sirenix.OdinInspector;
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
        [ShowInInspector] [ReadOnly] public string                  Id                     { get; [Preserve] private set; }
        [ShowInInspector]            public bool                    EnableTrigger          { get; [Preserve] private set; }
        [ShowInInspector]            public string                  NextStepId             { get; [Preserve] private set; }
        [ShowInInspector]            public List<string>            PreviousSteps          { get; [Preserve] private set; }
        [ShowInInspector]            public string                  ScreenLocation         { get; [Preserve] private set; }
        [ShowInInspector]            public List<string>            RequireTriggerComplete { get; [Preserve] private set; }
        [ShowInInspector]            public string                  RequireCondition       { get; [Preserve] private set; }
        [ShowInInspector]            public string                  HighLightPath          { get; [Preserve] private set; }
        [ShowInInspector]            public bool                    ButtonCanClick         { get; [Preserve] private set; }
        [ShowInInspector]            public float                   Radius                 { get; [Preserve] private set; }
        [ShowInInspector]            public string                  HandAnchor             { get; [Preserve] private set; }
        [ShowInInspector]            public Vector3                 HandRotation           { get; [Preserve] private set; }
        [ShowInInspector]            public Vector2                 HandSizeDelta          { get; [Preserve] private set; }
        [ShowInInspector]            public Dictionary<string, int> BonusOnStart           { get; [Preserve] private set; }
        [ShowInInspector]            public string                  TooltipText            { get; [Preserve] private set; }
        [ShowInInspector]            public float                   TooltipDuration        { get; [Preserve] private set; }
        [ShowInInspector]            public bool                    HideOnComplete         { get; [Preserve] private set; }

        public List<RequireCondition> GetRequireCondition()
        {
            return JsonConvert.DeserializeObject<List<RequireCondition>>(this.RequireCondition);
        }

        [Preserve]
        public UITemplateFTUERecord()
        {
        }

        [JsonConstructor]
        public UITemplateFTUERecord(
            string                  id,
            bool                    enableTrigger,
            string                  nextStepId,
            List<string>            previousSteps,
            string                  screenLocation,
            List<string>            requireTriggerComplete,
            string                  requireCondition,
            string                  highLightPath,
            bool                    buttonCanClick,
            float                   radius,
            string                  handAnchor,
            Vector3                 handRotation,
            Vector2                 handSizeDelta,
            Dictionary<string, int> bonusOnStart,
            string                  tooltipText,
            float                   tooltipDuration,
            bool                    hideOnComplete
        )
        {
            this.Id                     = id;
            this.EnableTrigger          = enableTrigger;
            this.NextStepId             = nextStepId;
            this.PreviousSteps          = previousSteps;
            this.ScreenLocation         = screenLocation;
            this.RequireTriggerComplete = requireTriggerComplete;
            this.RequireCondition       = requireCondition;
            this.HighLightPath          = highLightPath;
            this.ButtonCanClick         = buttonCanClick;
            this.Radius                 = radius;
            this.HandAnchor             = handAnchor;
            this.HandRotation           = handRotation;
            this.HandSizeDelta          = handSizeDelta;
            this.BonusOnStart           = bonusOnStart;
            this.TooltipText            = tooltipText;
            this.TooltipDuration        = tooltipDuration;
            this.HideOnComplete         = hideOnComplete;
        }
    }

    [Preserve]
    public class RequireCondition
    {
        [JsonProperty] public string RequireId       { get; [Preserve] private set; }
        [JsonProperty] public string ConditionDetail { get; [Preserve] private set; }
    }
}