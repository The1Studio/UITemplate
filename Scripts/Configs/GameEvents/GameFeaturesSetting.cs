namespace TheOneStudio.UITemplate.UITemplate.Configs.GameEvents
{
    using Sirenix.OdinInspector;
    using TheOneStudio.UITemplate.UITemplate.Services.Vibration;
    using UnityEngine;
#if UNITY_EDITOR
    using UnityEditor;
    using System.IO;
    using ServiceImplementation.Configs.Editor;
#endif

    [CreateAssetMenu(fileName = nameof(GameFeaturesSetting),
        menuName = "TheOne/ScriptableObjects/SpawnGameFeatruesSettingConfig", order = 1)]
    public class GameFeaturesSetting : ScriptableObject
    {
        private const string RacingEventSymbol      = "THEONE_RACING_EVENT";
        private const string DailyRewardSymbol      = "THEONE_DAILY_REWARD";
        private const string BadgeNotifySymbol      = "THEONE_BADGE_NOTIFY";
        private const string QuestSystemSymbol      = "THEONE_QUEST_SYSTEM";
        private const string FireBaseAuthSymbol     = "THEONE_FIREBASE_AUTH";
        private const string DailyQueueRewardSymbol = "THEONE_DAILY_QUEUE_REWARD";

        public static string ResourcePath = $"GameConfigs/{nameof(GameFeaturesSetting)}";

        #region Racing Event

        [OnValueChanged("OnChangeRacingEvent")]
        public bool enableRacingEvent;

        [SerializeField] [ShowIf("enableRacingEvent")] [BoxGroup("Racing Event")]
        public GameEventRacingConfig racingConfig;

        public GameEventRacingConfig RacingConfig => this.racingConfig;

        #endregion

        #region Daily Reward

        [OnValueChanged("OnChangeDailyReward")]
        public bool enableDailyReward;

        [SerializeField] [ShowIf(nameof(enableDailyReward))] [BoxGroup("Daily Reward")]
        private DailyRewardConfig dailyRewardConfig;

        public DailyRewardConfig DailyRewardConfig => this.dailyRewardConfig;

        #endregion

        #region Badge Notify

        [OnValueChanged("OnChangeBadgeNotify")]
        public bool enableBadgeNotify;

        #endregion

        #region Quest

        [OnValueChanged("OnChangeQuestSystem")]
        public bool enableQuestSystem;

        [SerializeField] [ShowIf(nameof(enableQuestSystem))] [BoxGroup("Quest System")]
        private QuestSystemConfig questSystemConfig;

        public QuestSystemConfig QuestSystemConfig => this.questSystemConfig;

        #endregion

        #region Misc

        [FoldoutGroup("Misc Settings")] public bool enableInitHomeScreenManually = true;
        [FoldoutGroup("Misc Settings")] public bool showBottomBarWithBanner = false;
        [FoldoutGroup("Misc Settings")] [Tooltip("Auto Request App Tracking Transparent for iOS")] public bool autoRequestATT = true;

        #endregion

        #region Button Experience

        [FoldoutGroup("Button Experience")] [Tooltip("Set to None to disable")]
        public VibrationPresetType vibrationPresetType = VibrationPresetType.Selection;

        [FoldoutGroup("Button Experience")] [Tooltip("Set to empty to disable")]
        public string clickButtonSound = "click_button";

        [FoldoutGroup("Button Experience")] public bool enableScaleAnimationOnCLicked = true;

        #endregion

        #region Firebase

        [OnValueChanged("OnChangeFirebaseAuth")] [BoxGroup("Firebase")]
        public bool enableFirebaseAuth;

        #endregion

        #region Daily Queue Reward

        [OnValueChanged("OnChangeDailyQueueReward")] [BoxGroup("Daily Queue Reward")]
        public bool enableDailyQueueReward;

        #endregion

#if UNITY_EDITOR
        private void OnChangeRacingEvent() { DefineSymbolEditorUtils.SetDefineSymbol(RacingEventSymbol, this.enableRacingEvent); }

        private void OnChangeDailyReward() { DefineSymbolEditorUtils.SetDefineSymbol(DailyRewardSymbol, this.enableDailyReward); }

        private void OnChangeBadgeNotify() { DefineSymbolEditorUtils.SetDefineSymbol(BadgeNotifySymbol, this.enableBadgeNotify); }

        private void OnChangeQuestSystem() { DefineSymbolEditorUtils.SetDefineSymbol(QuestSystemSymbol, this.enableQuestSystem); }

        private void OnChangeFirebaseAuth() { DefineSymbolEditorUtils.SetDefineSymbol(FireBaseAuthSymbol, this.enableFirebaseAuth); }

        private async void OnChangeDailyQueueReward()
        {
            const string filePath = "Assets/Resources/BlueprintData/UITemplateDailyQueueOffer.csv";
            DefineSymbolEditorUtils.SetDefineSymbol(DailyQueueRewardSymbol, this.enableDailyQueueReward);

            if (!this.enableDailyQueueReward) return;
            if (File.Exists(filePath)) return;

            await using (var writer = new StreamWriter(filePath, true))
            {
                await writer.WriteLineAsync("Day,OfferId,ItemId,ImageId,Value,IsRewardedAds");
            }

            AssetDatabase.Refresh();
        }
#endif
    }
}