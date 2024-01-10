namespace TheOneStudio.UITemplate.UITemplate.Configs.GameEvents
{
    using System;
    using System.Collections.Generic;
    using Sirenix.OdinInspector;

    [Serializable]
    public class DailyRewardConfig
    {
        public bool isCustomScreenTrigger;

        [ShowIf(nameof(isCustomScreenTrigger))]
        public List<string> screenTriggerIds;

        public bool isAutoCloseAfterClaim;
        public bool getNextDayWithAds;
        public bool showOnFirstOpen;

        [FoldoutGroup("Custom Id")] [ShowIf(nameof(getNextDayWithAds))]
        public string dailyRewardAdPlacementId = "DailyReward";

        [FoldoutGroup("Custom Id")] public string notificationId = "daily_reward";
    }
}