namespace TheOneStudio.UITemplate.UITemplate.Configs.GameEvents
{
    using System;
    using System.Collections.Generic;
    using ServiceImplementation.Configs.CustomTypes;
    using Sirenix.OdinInspector;

    [Serializable]
    public class DailyRewardConfig
    {
        public bool isCustomScreenTrigger;

        [ShowIf(nameof(isCustomScreenTrigger))]
        public List<string> screenTriggerIds;

        public PreReceiveDailyRewardStrategy preReceiveDailyRewardStrategy = PreReceiveDailyRewardStrategy.None;
        public bool                          showOnFirstOpen;

        private bool isCustomPreReceive => this.preReceiveDailyRewardStrategy == PreReceiveDailyRewardStrategy.Custom;
        
        [ShowIf(nameof(isCustomPreReceive))] public IntToBooleanSerializable preReceiveConfig;

        [FoldoutGroup("Custom Id")] public string dailyRewardAdPlacementId = "DailyReward";

        [FoldoutGroup("Custom Id")] public string notificationId = "daily_reward";
    }

    [Serializable]
    public class IntToBooleanSerializable : SerializableDictionary<int, bool>
    {
    }

    public enum PreReceiveDailyRewardStrategy
    {
        None,
        NextDay,
        Custom
    }
}