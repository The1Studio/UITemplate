namespace TheOneStudio.UITemplate.UITemplate.Models.LocalDatas
{
    using System;
    using GameFoundation.Scripts.Interfaces;
    using Sirenix.Serialization;

    public class UITemplateLuckySpinData : ILocalData
    {
        [OdinSerialize] public int      FreeTurns                { get; set; }
        [OdinSerialize] public bool     IsFirstTimeOpenLuckySpin { get; set; }
        public                 DateTime LastTimeGetFreeTurn      { get; set; }
        public                 DateTime LastSpinTime             { get; set; }

        public void Init()
        {
        }
    }
}