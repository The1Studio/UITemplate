namespace TheOneStudio.UITemplate.UITemplate.Models
{
    using System;
    using GameFoundation.Scripts.Interfaces;
    using Sirenix.OdinInspector;
    using Sirenix.Serialization;

    public class UITemplateUserJackpotData : ILocalData
    {
        [OdinSerialize]
        public int CurrentJackpotSpin { get; set; } = 0;

        [OdinSerialize]
        public int RemainingJackpotSpin { get; set; } = 100;

        public DateTime JackpotDate { get; set; } = DateTime.Now;

        public void Init()
        {
        }
    }
}