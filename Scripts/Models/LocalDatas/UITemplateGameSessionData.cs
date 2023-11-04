namespace TheOneStudio.UITemplate.UITemplate.Models.LocalDatas
{
    using System;
    using GameFoundation.Scripts.Interfaces;
    using Sirenix.Serialization;
    using TheOneStudio.UITemplate.UITemplate.Models.Controllers;

    public class UITemplateGameSessionData : ILocalData, IUITemplateLocalData
    {
        [OdinSerialize] public DateTime FirstInstallDate { get; set; } = DateTime.Now;

        public void Init() { }

        public Type ControllerType => typeof(UITemplateGameSessionDataController);
    }
}