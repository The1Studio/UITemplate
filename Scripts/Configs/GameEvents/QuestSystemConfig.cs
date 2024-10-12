namespace TheOneStudio.UITemplate.UITemplate.Configs.GameEvents
{
    using System;
    using Sirenix.OdinInspector;

    [Serializable]
    public class QuestSystemConfig
    {
        public int showNotificationAfterSession;

        public bool enableQuestClaimSound;

        [ShowIf(nameof(enableQuestClaimSound))] public string questClaimSoundKey;

        public bool enableQuestNotificationSoundKey;

        [ShowIf(nameof(enableQuestNotificationSoundKey))] public string questNotificationSoundKey;
    }
}