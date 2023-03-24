namespace TheOneStudio.UITemplate.UITemplate.Models.Controllers
{
    using System;
    using Cysharp.Threading.Tasks;
    using TheOneStudio.UITemplate.UITemplate.Services;

    public class UITemplateDailyRewardController
    {
        #region inject

        private readonly IInternetService          internetService;
        private readonly UITemplateDailyRewardData uiTemplateDailyRewardData;

        #endregion

        public UITemplateDailyRewardController(IInternetService internetService, UITemplateDailyRewardData uiTemplateDailyRewardData)
        {
            this.internetService           = internetService;
            this.uiTemplateDailyRewardData = uiTemplateDailyRewardData;
        }

        public async UniTask<int> GetUserLoginDay()
        {
            var currentDay = (await this.internetService.GetCurrentTimeAsync()).Day;
            var beginDay   = this.uiTemplateDailyRewardData.BeginDate.Day;

            return (currentDay - beginDay) > 0 ? (currentDay - beginDay) : 0;
        }

        public void SetRewardStatus(int day, RewardStatus status)
        {
            if (this.uiTemplateDailyRewardData.RewardStatus[day] == RewardStatus.Claimed)
                return;

            this.uiTemplateDailyRewardData.BeginDate = status == RewardStatus.Unlocked
                ? DateTime.Now
                : this.uiTemplateDailyRewardData.BeginDate;
            this.uiTemplateDailyRewardData.RewardStatus[day] = status;
        }

        public void ResetRewardStatus(int count)
        {
            for (var i = 0; i < count; i++)
            {
                this.uiTemplateDailyRewardData.RewardStatus.Add(RewardStatus.Locked);
            }

            this.uiTemplateDailyRewardData.BeginDate = DateTime.Now;
        }
    }
}