﻿namespace TheOneStudio.UITemplate.UITemplate.Models
{
    using System;
    using GameFoundation.Scripts.Interfaces;
    using Sirenix.Serialization;
    using TheOneStudio.UITemplate.UITemplate.Models.Controllers;
    using TheOneStudio.UITemplate.UITemplate.Models.LocalDatas;
    using UnityEngine.Scripting;

    [Preserve]
    public class UITemplateUserSettingData : ILocalData, IUITemplateLocalData
    {
        [OdinSerialize] public bool IsVibrationEnable = true;

        [OdinSerialize] public bool IsFlashLightEnable = true;

        public void Init()
        {
        }

        public Type ControllerType => typeof(UITemplateSettingDataController);
    }
}