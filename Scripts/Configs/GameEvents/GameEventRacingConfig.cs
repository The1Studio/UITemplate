namespace TheOneStudio.UITemplate.UITemplate.Configs.GameEvents
{
    using System;
    using System.Collections.Generic;

    [Serializable]
    public class GameEventRacingConfig
    {
        public string RacingCurrency;
        public int    RacingScoreMax              = 1000;
        public float  RacingMaxProgressionPercent = 0.8f;
        public int    RacingDay                   = 7;

        public List<string> IconAddressableSet;
    }
}