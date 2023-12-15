namespace TheOneStudio.UITemplate.UITemplate.Configs.GameEvents
{
    using System;
    using System.Collections.Generic;
    using TheOneStudio.UITemplate.UITemplate.Utils;

    [Serializable]
    public class GameEventRacingConfig
    {
        public string                RacingCurrency;
        public int                   RacingScoreMax              = 1000;
        public float                 RacingMaxProgressionPercent = 0.8f;
        public int                   RacingDay                   = 7;
        public StringToIntDictionary RewardValue;

        public List<string> IconAddressableSet;
    }
}