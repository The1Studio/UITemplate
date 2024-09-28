namespace TheOneStudio.UITemplate.UITemplate.Models.LocalDatas
{
    using System;
    using GameFoundation.Scripts.Interfaces;
    using TheOneStudio.UITemplate.UITemplate.Models.Controllers;
    using UnityEngine.Scripting;

    [Preserve]
    public class UITemplateCommonData : ILocalData,IUITemplateLocalData
    {
        public bool IsFirstTimeOpenGame { get; set; } = true;
        public Type ControllerType      => typeof(UITemplateCommonController);
        public void Init()              { }
    }
}