namespace TheOneStudio.UITemplate.UITemplate.Configs.GameEvents
{
    using System;
    using Sirenix.OdinInspector;

    [Serializable]
    public class QuestSystemConfig
    {
        public int showNotificationAfterSession;

        public bool   enableQuestClaimSound;
        
        [ShowIf(nameof(QuestSystemConfig.enableQuestClaimSound))]
        public string questClaimSoundKey;

        public bool enableQuestNotificationSoundKey;
        
        [ShowIf(nameof(QuestSystemConfig.enableQuestNotificationSoundKey))]
        public string questNotificationSoundKey;
    }
}