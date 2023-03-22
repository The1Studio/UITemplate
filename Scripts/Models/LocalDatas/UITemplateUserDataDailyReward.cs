namespace TheOneStudio.UITemplate.UITemplate.Models
{
    using System;
    using System.Collections.Generic;
    using GameFoundation.Scripts.Interfaces;

    public class UITemplateDailyRewardData : ILocalData
    {
        internal List<RewardStatus> RewardStatus { get; set; } = new();
        internal DateTime           BeginDate    { get; set; }

        public void Init() { }
    }

    public enum RewardStatus
    {
        Locked   = 0,
        Unlocked = 1,
        Claimed  = 2,
    }
}