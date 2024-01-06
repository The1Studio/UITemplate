namespace TheOneStudio.UITemplate.UITemplate.Configs.GameEvents
{
    using System.Collections.Generic;
    using Sirenix.OdinInspector;

    [System.Serializable]
    public class DailyRewardConfig
    {
        public bool isCustomScreenTrigger;

        [ShowIf(nameof(isCustomScreenTrigger))]
        public List<string> screenTriggerIds;

        public bool isGetNextDayWithAds;
    }
}