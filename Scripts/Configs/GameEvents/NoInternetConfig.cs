namespace TheOneStudio.UITemplate.UITemplate.Configs.GameEvents
{
    using System;
    using System.Collections.Generic;
    using Sirenix.OdinInspector;

    [Serializable] public class NoInternetConfig
    {
        public bool isCustomScreenTrigger;

        [ShowIf(nameof(isCustomScreenTrigger))]
        public List<string> screenTriggerIds;

        public int   SessionToShow       = 2;
        public int   ContinuesFailToShow = 3;
        public float DelayToCheck        = 30f;
        public float CheckInterval       = 1f;
    }
}