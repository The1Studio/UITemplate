namespace TheOneStudio.UITemplate.UITemplate.Models.LocalDatas
{
    using System;
    using System.Collections.Generic;
    using GameFoundation.Scripts.Interfaces;
    using Newtonsoft.Json;
    using TheOneStudio.UITemplate.UITemplate.Models.Controllers;
    using UnityEngine.Scripting;

    [Preserve]
    public class UITemplateIAPOwnerPackData : ILocalData, IUITemplateLocalData
    {
        [JsonProperty] internal List<string> OwnedPacks         { get; set; } = new();
        [JsonProperty] internal bool         IsRestoredPurchase { get; set; }

        public void Init() { }

        public Type ControllerType => typeof(UITemplateIAPOwnerPackControllerData);
    }
}