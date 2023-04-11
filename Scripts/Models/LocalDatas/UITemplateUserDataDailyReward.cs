namespace TheOneStudio.UITemplate.UITemplate.Models
{
    using System;
    using System.Collections.Generic;
    using GameFoundation.Scripts.Interfaces;
    using Sirenix.Serialization;


    public class UITemplateDailyRewardData : ILocalData
    {
        [OdinSerialize] public List<RewardStatus> RewardStatus = new();
        [OdinSerialize] public DateTime           LastRewardedDate    { get; set; }
        [OdinSerialize] public DateTime           FirstTimeOpenedDate { get; set; } = DateTime.Now;

        public void Init() { }
    }

    public enum RewardStatus
    {
        Locked   = 0,
        Unlocked = 1,
        Claimed  = 2,
    }
}