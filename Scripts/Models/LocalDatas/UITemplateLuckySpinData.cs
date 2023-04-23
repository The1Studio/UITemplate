namespace TheOneStudio.UITemplate.UITemplate.Models.LocalDatas
{
    using System;
    using GameFoundation.Scripts.Interfaces;
    using Sirenix.Serialization;
    using TheOneStudio.UITemplate.UITemplate.Models.Controllers;

    public class UITemplateLuckySpinData : ILocalData,IUITemplateLocalData
    {
        [OdinSerialize] public bool     IsFirstTimeOpenLuckySpin { get; set; }
        public                 DateTime LastTimeGetFreeTurn      { get; set; }
        public                 DateTime LastSpinTime             { get; set; }

        public void Init()
        {
        }

        public Type ControllerType => typeof(UITemplateLuckySpinController);
    }
}