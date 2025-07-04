﻿namespace TheOneStudio.UITemplate.UITemplate.Models.LocalDatas
{
    using System;
    using GameFoundation.Scripts.Interfaces;
    using Newtonsoft.Json;
    using Sirenix.Serialization;
    using TheOneStudio.UITemplate.UITemplate.Models.Controllers;
    using UnityEngine.Scripting;

    [Preserve]
    public class UITemplateGameSessionData : ILocalData, IUITemplateLocalData
    {
        [JsonProperty] [OdinSerialize] internal DateTime FirstInstallDate { get; set; } = DateTime.Now;
        [JsonProperty]                 internal int      OpenTime;

        public void Init() { }

        public Type ControllerType => typeof(UITemplateGameSessionDataController);
    }
}