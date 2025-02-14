namespace TheOneStudio.UITemplate.UITemplate.FTUE.RemoteConfig
{
    using System;
    using System.Collections.Generic;
    using Newtonsoft.Json;
    using Sirenix.OdinInspector;
    using TheOneStudio.UITemplate.UITemplate.Blueprints;
    using UnityEngine;

    [Serializable]
    public class FTUERecordDTO
    {
        [ShowInInspector]                                   private bool                    IsShowFull = false;
        [ShowInInspector] [ShowIf("IsShowFull")] [ReadOnly] public  string                  Id;
        [ShowInInspector] [ShowIf("IsShowFull")]            public  bool                    EnableTrigger;
        [ShowInInspector] [ShowIf("IsShowFull")]            public  string                  NextStepId;
        [ShowInInspector] [ShowIf("IsShowFull")]            public  List<string>            PreviousSteps;
        [ShowInInspector] [ShowIf("IsShowFull")]            public  string                  ScreenLocation;
        [ShowInInspector] [ShowIf("IsShowFull")]            public  List<string>            RequireTriggerComplete;
        [ShowInInspector] [ShowIf("IsShowFull")]            public  string                  RequireCondition;
        [ShowInInspector] [ShowIf("IsShowFull")]            public  string                  HighLightPath;
        [ShowInInspector] [ShowIf("IsShowFull")]            public  bool                    ButtonCanClick;
        [ShowInInspector] [ShowIf("IsShowFull")]            public  float                   Radius;
        [ShowInInspector] [ShowIf("IsShowFull")]            public  string                  HandAnchor;
        [ShowInInspector] [ShowIf("IsShowFull")]            public  Vector3                 HandRotation;
        [ShowInInspector] [ShowIf("IsShowFull")]            public  Vector2                 HandSizeDelta;
        [ShowInInspector] [ShowIf("IsShowFull")]            public  Dictionary<string, int> BonusOnStart;
        [ShowInInspector] [ShowIf("IsShowFull")]            public  string                  TooltipText;
        [ShowInInspector] [ShowIf("IsShowFull")]            public  float                   TooltipDuration;
        [ShowInInspector] [ShowIf("IsShowFull")]            public  bool                    HideOnComplete;

        public List<RequireCondition> GetRequireCondition()
        {
            return JsonConvert.DeserializeObject<List<RequireCondition>>(this.RequireCondition);
        }
    }

    public static class FTUERecordDTOExtensions
    {
        public static FTUERecordDTO ToDTO(this UITemplateFTUERecord record)
        {
            return new()
            {
                Id                     = record.Id,
                EnableTrigger          = record.EnableTrigger,
                NextStepId             = record.NextStepId,
                PreviousSteps          = record.PreviousSteps,
                ScreenLocation         = record.ScreenLocation,
                RequireTriggerComplete = record.RequireTriggerComplete,
                RequireCondition       = record.RequireCondition,
                HighLightPath          = record.HighLightPath,
                ButtonCanClick         = record.ButtonCanClick,
                Radius                 = record.Radius,
                HandAnchor             = record.HandAnchor,
                HandRotation           = record.HandRotation,
                HandSizeDelta          = record.HandSizeDelta,
                BonusOnStart           = record.BonusOnStart,
                TooltipText            = record.TooltipText,
                TooltipDuration        = record.TooltipDuration,
                HideOnComplete         = record.HideOnComplete
            };
        }
    }
}