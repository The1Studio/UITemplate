namespace TheOneStudio.UITemplate.UITemplate.Models
{
    using System;
    using System.Collections.Generic;
    using Cysharp.Threading.Tasks;
    using TheOneStudio.UITemplate.UITemplate.Services;

    public class UITemplateDailyRewardData
    {
        private readonly IInternetService internetService;

        public UITemplateDailyRewardData() { this.internetService = new InternetService(); }

        public       List<RewardStatus> RewardStatus = new();
        public       DateTime           BeginDate         { get; set; }
        public async UniTask<int>       GetUserLoginDay() => (await this.internetService.GetCurrentTimeAsync()).Day - this.BeginDate.Day + 1;
    }

    public enum RewardStatus
    {
        Locked   = 0,
        Unlocked = 1,
        Claimed  = 2,
    }
}