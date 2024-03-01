namespace TheOneStudio.UITemplate.UITemplate.Configs.GameEvents
{
    using Sirenix.OdinInspector;
    using TheOneStudio.UITemplate.UITemplate.Services.Vibration;
    using UnityEngine;
#if UNITY_EDITOR
    using ServiceImplementation.Configs.Editor;
#endif

    [CreateAssetMenu(fileName = nameof(GameFeaturesSetting),
        menuName = "TheOne/ScriptableObjects/SpawnGameFeatruesSettingConfig", order = 1)]
    public class GameFeaturesSetting : ScriptableObject
    {
        private const string RacingEventSymbol = "THEONE_RACING_EVENT";
        private const string DailyRewardSymbol = "THEONE_DAILY_REWARD";
        private const string BadgeNotifySymbol = "THEONE_BADGE_NOTIFY";
        private const string QuestSystemSymbol = "THEONE_QUEST_SYSTEM";

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

        #endregion

        #region Button Experience

        [FoldoutGroup("Button Experience")] [Tooltip("Set to None to disable")]
        public VibrationPresetType vibrationPresetType = VibrationPresetType.SoftImpact;

        [FoldoutGroup("Button Experience")] [Tooltip("Set to empty to disable")]
        public string clickButtonSound = "click_button";

        [FoldoutGroup("Button Experience")] public bool enableScaleAnimationOnCLicked = true;

        #endregion

#if UNITY_EDITOR
        private void OnChangeRacingEvent() { DefineSymbolEditorUtils.SetDefineSymbol(RacingEventSymbol, this.enableRacingEvent); }

        private void OnChangeDailyReward() { DefineSymbolEditorUtils.SetDefineSymbol(DailyRewardSymbol, this.enableDailyReward); }

        private void OnChangeBadgeNotify() { DefineSymbolEditorUtils.SetDefineSymbol(BadgeNotifySymbol, this.enableBadgeNotify); }

        private void OnChangeQuestSystem() { DefineSymbolEditorUtils.SetDefineSymbol(QuestSystemSymbol, this.enableQuestSystem); }
#endif
    }
}