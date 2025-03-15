namespace UITemplate.Scripts.Signals
{
    using System;
    using UnityEngine;

    public class PlayCurrencyAnimationSignal
    {
        public string        currecyId                  = null;
        public int           amount                     = 0;
        public int           currencyWithCap            = 0;
        public RectTransform startAnimationRect         = null;
        public string        claimSoundKey              = null;
        public string        flyCompleteSoundKey        = null;
        public int           minAnimAmount              = 6;
        public int           maxAnimAmount              = 10;
        public float         timeAnimAnim               = 1f;
        public float         flyPunchPositionAnimFactor = 0.3f;
        public Action        onCompleteEachItem         = null;
    }
}