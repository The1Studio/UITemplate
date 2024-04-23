namespace TheOneStudio.UITemplate.UITemplate.Configs.GameEvents
{
    using System;
    using System.Collections.Generic;
    using Sirenix.OdinInspector;
    using UnityEngine;

    [Serializable]
    public class RateUsConfig
    {
        [Tooltip("Set this to false if you want to use custom logic for showing rate us popup")]
        public bool isUsingCommonLogic = true;

        [ShowIf(nameof(isUsingCommonLogic))] [Tooltip("Set this to true if you want to show rate us popup only on specific screens")]
        public bool isCustomScreenTrigger = false;

        [ShowIf(nameof(isCustomScreenTrigger))] [ShowIf(nameof(isUsingCommonLogic))]
        public List<string> screenTriggerIds;

        [Tooltip("Set this to lower or equal 0 if you want to show the rate us popup immediately")] [ShowIf(nameof(isUsingCommonLogic))]
        public int SessionToShow = 2;

        [ShowIf(nameof(isCustomScreenTrigger))] [ShowIf(nameof(isUsingCommonLogic))]
        public int DelayInSecondsTillShow = 60;
    }
}