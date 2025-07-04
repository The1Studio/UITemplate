namespace TheOneStudio.UITemplate.UITemplate.Models
{
    using System;
    using System.Collections.Generic;
    using GameFoundation.Scripts.Interfaces;
    using Newtonsoft.Json;
    using Sirenix.Serialization;
    using TheOneStudio.UITemplate.UITemplate.Models.Controllers;
    using TheOneStudio.UITemplate.UITemplate.Models.LocalDatas;
    using UnityEngine.Scripting;

    [Preserve]
    public class UITemplateInventoryData : ILocalData, IUITemplateLocalData
    {
        [JsonProperty] [OdinSerialize] internal readonly Dictionary<string, string> CategoryToChosenItem = new();

        [JsonProperty] [OdinSerialize] internal Dictionary<string, UITemplateItemData> IDToItemData { get; private set; } = new();

        [JsonProperty] [OdinSerialize] internal Dictionary<string, UITemplateCurrencyData> IDToCurrencyData { get; private set; } = new();

        public Type ControllerType => typeof(UITemplateInventoryDataController);

        public void Init() { }
    }
}