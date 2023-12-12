namespace ServiceImplementation.Configs.GameEvents
{
    using System.Collections.Generic;
    using UnityEngine;

    [CreateAssetMenu(fileName = nameof(GameEventRacingConfig), menuName = "TheOne/ScriptableObjects/SpawnGameEventRacingConfig", order = 1)]
    public class GameEventRacingConfig : ScriptableObject
    {
        public string          RacingCurrency;
        public HashSet<string> IconAddressableSet;
    }
}