namespace TheOneStudio.UITemplate.UITemplate.Models.LocalDatas
{
    using System;
    using GameFoundation.Scripts.Interfaces;
    using Sirenix.Serialization;

    public class UITemplateLuckySpinData : ILocalData
    {
        [OdinSerialize]
        public bool IsUsedFreeSpin { get; set; }

        [OdinSerialize]
        public bool IsFirstTimeOpenLuckySpin { get; set; }

        public DateTime LastSpinTime { get; set; }

        public void Init()
        {
        }
    }
}