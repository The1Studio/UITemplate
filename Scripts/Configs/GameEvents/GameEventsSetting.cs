namespace TheOneStudio.UITemplate.UITemplate.Configs.GameEvents
{
    using UnityEngine;

    [CreateAssetMenu(fileName = nameof(GameEventsSetting), menuName = "TheOne/ScriptableObjects/SpawnGameEventSettingConfig", order = 1)]
    public class GameEventsSetting : ScriptableObject
    {
        public static string ResourcePath = $"GameConfigs/{nameof(GameEventsSetting)}";

        public        GameEventRacingConfig RacingConfig;
    }
}