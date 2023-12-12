namespace ServiceImplementation.Configs.GameEvents
{
    using UnityEngine;

    [CreateAssetMenu(fileName = nameof(GameEventRacingConfig), menuName = "TheOne/ScriptableObjects/SpawnGameEventRacingConfig", order = 1)]
    public class GameEventRacingConfig : ScriptableObject
    {
        public string RacingCurrency;
    }
}