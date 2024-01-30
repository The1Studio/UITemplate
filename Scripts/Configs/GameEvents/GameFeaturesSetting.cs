namespace TheOneStudio.UITemplate.UITemplate.Configs.GameEvents
{
#if UNITY_EDITOR
    using ServiceImplementation.Configs.Editor;
#endif
    using System;
    using Sirenix.OdinInspector;
    using UnityEngine;

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

#if UNITY_EDITOR
        private void OnChangeRacingEvent() { DefineSymbolEditorUtils.SetDefineSymbol(RacingEventSymbol, this.enableRacingEvent); }

        private void OnChangeDailyReward() { DefineSymbolEditorUtils.SetDefineSymbol(DailyRewardSymbol, this.enableDailyReward); }

        private void OnChangeBadgeNotify() { DefineSymbolEditorUtils.SetDefineSymbol(BadgeNotifySymbol, this.enableBadgeNotify); }
        
        private void OnChangeQuestSystem() { DefineSymbolEditorUtils.SetDefineSymbol(QuestSystemSymbol, this.enableQuestSystem); }
#endif
    }
}