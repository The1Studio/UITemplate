namespace TheOneStudio.UITemplate.UITemplate.Models
{
    using System.Collections.Generic;
    using GameFoundation.Scripts.Interfaces;
    using Newtonsoft.Json;
    using Sirenix.Serialization;
    using UnityEngine.Scripting;

    [Preserve]
    public class UITemplateFTUEData : ILocalData
    {
        [JsonProperty] [OdinSerialize] internal List<string> FinishedStep { get; set; } = new();
        [JsonProperty] [OdinSerialize] internal List<string> RewardedStep { get; set; } = new();

        public void Init() { }
    }
}