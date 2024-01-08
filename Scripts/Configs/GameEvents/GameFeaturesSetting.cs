namespace TheOneStudio.UITemplate.UITemplate.Configs.GameEvents
{
#if UNITY_EDITOR
    using ServiceImplementation.Configs.Editor;
#endif
    using Sirenix.OdinInspector;
    using UnityEngine;

    [CreateAssetMenu(fileName = nameof(GameFeaturesSetting),
        menuName = "TheOne/ScriptableObjects/SpawnGameFeatruesSettingConfig", order = 1)]
    public class GameFeaturesSetting : ScriptableObject
    {
        private const string RacingEventSymbol = "THEONE_RACING_EVENT";
        private const string DailyRewardSymbol = "THEONE_DAILY_REWARD";
        private const string BadgeNotifySymbol = "THEONE_BADGE_NOTIFY";


        public static string ResourcePath = $"GameConfigs/{nameof(GameFeaturesSetting)}";

        [OnValueChanged("OnChangeRacingEvent")]
        public bool enableRacingEvent;

        [SerializeField] [ShowIf("enableRacingEvent")] [BoxGroup("Racing Event")]
        public GameEventRacingConfig racingConfig;

        public GameEventRacingConfig RacingConfig => this.racingConfig;
        
        [OnValueChanged("OnChangeDailyReward")]
        public bool enableDailyReward;
        
        [OnValueChanged("OnChangeBadgeNotify")]
        public bool enableBadgeNotify;

#if UNITY_EDITOR
        private void OnChangeRacingEvent()
        {
            DefineSymbolEditorUtils.SetDefineSymbol(RacingEventSymbol, this.enableRacingEvent);
        }
        
        private void OnChangeDailyReward()
        {
            DefineSymbolEditorUtils.SetDefineSymbol(DailyRewardSymbol, this.enableDailyReward);
        }
        
        private void OnChangeBadgeNotify()
        {
            DefineSymbolEditorUtils.SetDefineSymbol(BadgeNotifySymbol, this.enableBadgeNotify);
        }
#endif
    }
}