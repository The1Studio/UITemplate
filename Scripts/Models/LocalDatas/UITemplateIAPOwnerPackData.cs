namespace TheOneStudio.UITemplate.UITemplate.Models.LocalDatas
{
    using System;
    using System.Collections.Generic;
    using GameFoundation.Scripts.Interfaces;
    using TheOneStudio.UITemplate.UITemplate.Models.Controllers;
    using UnityEngine.Scripting;

    [Preserve]
    public class UITemplateIAPOwnerPackData : ILocalData, IUITemplateLocalData
    {
        internal List<string> OwnedPacks { get; set; } = new();

        internal bool IsRestoredPurchase { get; set; }

        public void Init()
        {
        }

        public Type ControllerType => typeof(UITemplateIAPOwnerPackControllerData);
    }
}