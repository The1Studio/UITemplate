﻿namespace TheOneStudio.UITemplate.UITemplate.Models
{
    using System;
    using GameFoundation.Scripts.Interfaces;
    using Newtonsoft.Json;
    using Sirenix.Serialization;
    using TheOneStudio.UITemplate.UITemplate.Models.Controllers;
    using TheOneStudio.UITemplate.UITemplate.Models.LocalDatas;
    using UnityEngine.Scripting;

    [Preserve]
    public class UITemplateUserSettingData : ILocalData, IUITemplateLocalData
    {
        [JsonProperty] [OdinSerialize] internal bool IsVibrationEnable  = true;
        [JsonProperty] [OdinSerialize] internal bool IsFlashLightEnable = true;

        public void Init() { }

        public Type ControllerType => typeof(UITemplateSettingDataController);
    }
}