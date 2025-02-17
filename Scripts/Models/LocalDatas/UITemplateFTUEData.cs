namespace TheOneStudio.UITemplate.UITemplate.Models
{
    using System.Collections.Generic;
    using GameFoundation.Scripts.Interfaces;
    using Sirenix.Serialization;
    using UnityEngine.Scripting;

    [Preserve]
    public class UITemplateFTUEData : ILocalData
    {
        [OdinSerialize] public List<string> FinishedStep { get; set; } = new();
        [OdinSerialize] public List<string> RewardedStep { get; set; } = new();

        [OdinSerialize] public string CurrentStep { get; set; } = "";

        public void Init()
        {
        }
    }
}