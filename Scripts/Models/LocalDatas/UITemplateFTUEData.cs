namespace TheOneStudio.UITemplate.UITemplate.Models
{
    using System;
    using System.Collections.Generic;
    using GameFoundation.Scripts.Interfaces;
    using Sirenix.Serialization;
    using TheOneStudio.UITemplate.UITemplate.Models.Controllers;
    using TheOneStudio.UITemplate.UITemplate.Models.LocalDatas;
    using UnityEngine.Scripting;

    [Preserve]
    public class UITemplateFTUEData : ILocalData, IUITemplateLocalData
    {
        [OdinSerialize] public List<string> FinishedStep { get; set; } = new();

        [OdinSerialize] public string CurrentStep { get; set; } = "";

        public void Init() { }

        public Type ControllerType => typeof(UITemplateFTUEDataController);
    }
}