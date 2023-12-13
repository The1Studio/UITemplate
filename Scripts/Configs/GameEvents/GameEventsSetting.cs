namespace TheOneStudio.UITemplate.UITemplate.Configs.GameEvents
{
#if UNITY_EDITOR
    using ServiceImplementation.Configs.Editor;
#endif
    using Sirenix.OdinInspector;
    using UnityEngine;

    [CreateAssetMenu(fileName = nameof(GameEventsSetting),
        menuName = "TheOne/ScriptableObjects/SpawnGameEventSettingConfig", order = 1)]
    public class GameEventsSetting : ScriptableObject
    {
        private const string RacingEventSymbol = "THEONE_RACING_EVENT";

        public static string ResourcePath = $"GameConfigs/{nameof(GameEventsSetting)}";

        [OnValueChanged("OnChangeRacingEvent")]
        public bool enableRacingEvent;

        [SerializeField] [ShowIf("enableRacingEvent")] [BoxGroup("Racing Event")]
        public GameEventRacingConfig racingConfig;

        public GameEventRacingConfig RacingConfig => this.racingConfig;

#if UNITY_EDITOR
        private void OnChangeRacingEvent()
        {
            DefineSymbolEditorUtils.SetDefineSymbol(RacingEventSymbol, this.enableRacingEvent);
        }
#endif
    }
}