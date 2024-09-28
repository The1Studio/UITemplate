namespace TheOneStudio.UITemplate.UITemplate.Models.LocalDatas
{
    using System;
    using GameFoundation.Scripts.Interfaces;
    using Sirenix.Serialization;
    using TheOneStudio.UITemplate.UITemplate.Models.Controllers;
    using UnityEngine.Scripting;

    [Preserve]
    public class UITemplateGameSessionData : ILocalData, IUITemplateLocalData
    {
        [OdinSerialize] public DateTime FirstInstallDate { get; set; } = DateTime.Now;
        public                 int      OpenTime;

        public void Init() { }

        public Type ControllerType => typeof(UITemplateGameSessionDataController);
    }
}