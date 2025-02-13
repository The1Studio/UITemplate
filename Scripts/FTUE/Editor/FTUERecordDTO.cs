namespace UITemplate.Scripts.FTUE.Editor
{
    using System;
    using System.Collections.Generic;
    using Sirenix.OdinInspector;
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
    }
}