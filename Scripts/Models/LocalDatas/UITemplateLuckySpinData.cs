namespace TheOneStudio.UITemplate.UITemplate.Models.LocalDatas
{
    using System;
    using GameFoundation.Scripts.Interfaces;

    public class UITemplateLuckySpinData : ILocalData
    {
        public bool     IsUsedFreeSpin              { get; set; }
        public bool     IsFirstTimeOpenLuckySpin    { get; set; }
        public DateTime LastSpinTime                { get; set; }

        public void Init() { }
    }
}