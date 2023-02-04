namespace UITemplate.Scripts.Models
{
    using System;
    using Cysharp.Threading.Tasks;
    using Utility;

    public class UITemplateDailyRewardData
    {
        private readonly IInternetService internetService = new InternetService();

        public       DateTime     BeginDate         { get; set; }
        public       int          LoginDay          { get; set; }
        public async UniTask<int> GetUserLoginDay() => (await this.internetService.GetCurrentTimeAsync()).Day - this.BeginDate.Day + 1;
    }
}