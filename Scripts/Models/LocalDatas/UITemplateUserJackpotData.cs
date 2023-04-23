namespace TheOneStudio.UITemplate.UITemplate.Models
{
    using System;
    using GameFoundation.Scripts.Interfaces;
    using Sirenix.OdinInspector;
    using Sirenix.Serialization;
    using TheOneStudio.UITemplate.UITemplate.Models.Controllers;
    using TheOneStudio.UITemplate.UITemplate.Models.LocalDatas;

    public class UITemplateUserJackpotData : ILocalData, IUITemplateLocalData
    {
        [OdinSerialize] public int CurrentJackpotSpin { get; set; } = 0;

        [OdinSerialize] public int RemainingJackpotSpin { get; set; } = 100;

        public DateTime JackpotDate { get; set; } = DateTime.Now;

        public void Init() { }

        public Type ControllerType => typeof(UITemplateJackpotController);
    }
}