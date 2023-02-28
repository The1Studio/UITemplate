namespace TheOneStudio.UITemplate.UITemplate.Models
{
    using System;
    using System.Collections.Generic;
    using GameFoundation.Scripts.Interfaces;

    public class UITemplateDailyRewardData : ILocalData
    {
        public       List<RewardStatus> RewardStatus = new();
        public       DateTime           BeginDate         { get; set; }
       

        public void Init() { }
    }

    public enum RewardStatus
    {
        Locked   = 0,
        Unlocked = 1,
        Claimed  = 2,
    }
}