namespace TheOneStudio.UITemplate.UITemplate.Models.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Cysharp.Threading.Tasks;
    using TheOneStudio.UITemplate.UITemplate.Blueprints;
    using TheOneStudio.UITemplate.UITemplate.Services;
    using UnityEngine;

    public class UITemplateDailyRewardController
    {
        #region inject

        private readonly IInternetService               internetService;
        private readonly UITemplateDailyRewardData      uiTemplateDailyRewardData;
        private readonly UITemplateDailyRewardBlueprint uiTemplateDailyRewardBlueprint;

        #endregion

        public UITemplateDailyRewardController(IInternetService internetService, UITemplateDailyRewardData uiTemplateDailyRewardData,
            UITemplateDailyRewardBlueprint uiTemplateDailyRewardBlueprint)
        {
            this.internetService                = internetService;
            this.uiTemplateDailyRewardData      = uiTemplateDailyRewardData;
            this.uiTemplateDailyRewardBlueprint = uiTemplateDailyRewardBlueprint;
        }

        public async UniTask<int> GetUserLoginDay()
        {
            var currentDay      = (await this.internetService.GetCurrentTimeAsync()).Day;
            var beginDay        = this.uiTemplateDailyRewardData.BeginDate.Day;
            var differenceOfDay = currentDay - beginDay;

            return differenceOfDay > 0
                ? differenceOfDay >= this.uiTemplateDailyRewardData.RewardStatus.Count
                    ? this.uiTemplateDailyRewardData.RewardStatus.Count - 1
                    : differenceOfDay
                : 0;
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

        public void ClaimAllAvailableReward()
        {
            for (var i = 0; i < this.uiTemplateDailyRewardData.RewardStatus.Count; i++)
            {
                if (this.uiTemplateDailyRewardData.RewardStatus[i] == RewardStatus.Unlocked)
                    this.uiTemplateDailyRewardData.RewardStatus[i] = RewardStatus.Claimed;
            }
        }

        public void ResetRewardStatus()
        {
            for (var i = 0; i < this.uiTemplateDailyRewardBlueprint.Values.Count; i++)
            {
                this.uiTemplateDailyRewardData.RewardStatus.Add(RewardStatus.Locked);
            }

            this.uiTemplateDailyRewardData.RewardStatus[0] = RewardStatus.Unlocked;
            this.uiTemplateDailyRewardData.BeginDate       = DateTime.Now;
        }

        public bool CanClaimReward => this.uiTemplateDailyRewardData.RewardStatus.Any(t => t == RewardStatus.Unlocked);

        public List<UITemplateDailyRewardRecord> ListRewardBlueprint => this.uiTemplateDailyRewardBlueprint.Values.ToList();

        public bool IsFirstOpenGame()
        {
            if (PlayerPrefs.GetInt("FirstOpenApp") != 0) return false;
            PlayerPrefs.SetInt("FirstOpenApp", 1);
            return true;

        }
    }
}