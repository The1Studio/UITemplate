namespace TheOneStudio.UITemplate.UITemplate.Models.LocalDatas
{
    using System;
    using System.Collections.Generic;
    using GameFoundation.Scripts.Interfaces;
    using Newtonsoft.Json;
    using Sirenix.Serialization;
    using TheOneStudio.UITemplate.UITemplate.Models.Controllers;
    using UnityEngine.Scripting;

    [Preserve]
    public class UITemplateDailyRewardData : ILocalData, IUITemplateLocalData
    {
        [JsonProperty] [OdinSerialize] internal List<RewardStatus> RewardStatus        { get; set; } = new();
        [JsonProperty] [OdinSerialize] internal DateTime           LastRewardedDate    { get; set; }
        [JsonProperty] [OdinSerialize] internal DateTime           FirstTimeOpenedDate { get; set; } = DateTime.Now;

        public void Init() { }

        public Type ControllerType => typeof(UITemplateDailyRewardController);
    }

    public enum RewardStatus
    {
        Locked   = 0,
        Unlocked = 1,
        Claimed  = 2,
    }
}