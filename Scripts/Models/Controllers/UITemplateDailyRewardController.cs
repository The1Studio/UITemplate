namespace TheOneStudio.UITemplate.UITemplate.Models.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Cysharp.Threading.Tasks;
    using GameFoundation.Scripts.Utilities.Extension;
    using TheOneStudio.UITemplate.UITemplate.Blueprints;
    using TheOneStudio.UITemplate.UITemplate.Services;

    public class UITemplateDailyRewardController
    {
        private const int TotalDayInWeek = 7;

        #region inject

        private readonly IInternetService                  internetService;
        private readonly UITemplateDailyRewardData         uiTemplateDailyRewardData;
        private readonly UITemplateDailyRewardBlueprint    uiTemplateDailyRewardBlueprint;
        private readonly UITemplateInventoryDataController uiTemplateInventoryDataController;

        #endregion

        public UITemplateDailyRewardController(IInternetService internetService, UITemplateDailyRewardData uiTemplateDailyRewardData, UITemplateDailyRewardBlueprint uiTemplateDailyRewardBlueprint,
                                               UITemplateInventoryDataController uiTemplateInventoryDataController)
        {
            this.internetService                   = internetService;
            this.uiTemplateDailyRewardData         = uiTemplateDailyRewardData;
            this.uiTemplateDailyRewardBlueprint    = uiTemplateDailyRewardBlueprint;
            this.uiTemplateInventoryDataController = uiTemplateInventoryDataController;
        }

        public async UniTask CheckRewardStatus()
        {
            var currentTimeAsync = await this.internetService.GetCurrentTimeAsync();
            var diffDay          = (currentTimeAsync - this.uiTemplateDailyRewardData.LastRewardedDate).TotalDays;

            if (!(diffDay >= 1)) return;

            var firstLockedDayIndex = this.FindFirstLockedDayIndex();
            if (firstLockedDayIndex == -1)
            {
                if (!this.CanClaimReward)
                {
                    this.InitRewardStatus();
                }
            }
            else
            {
                if (firstLockedDayIndex / TotalDayInWeek == (firstLockedDayIndex) / TotalDayInWeek)
                {
                    this.uiTemplateDailyRewardData.RewardStatus[firstLockedDayIndex] = RewardStatus.Unlocked;
                    this.uiTemplateDailyRewardData.LastRewardedDate                  = await this.internetService.GetCurrentTimeAsync();
                }
            }
        }

        private int FindFirstLockedDayIndex()
        {
            return this.uiTemplateDailyRewardData.RewardStatus.FirstIndex(status => status is RewardStatus.Locked);
        }

        public int GetCurrentDayIndex()
        {
            var firstLockedDayIndex = this.FindFirstLockedDayIndex();
            return firstLockedDayIndex == -1 ? this.uiTemplateDailyRewardData.RewardStatus.Count - 1 : firstLockedDayIndex - 1;
        }

        /// <param name="day"> start from 1</param>
        /// <returns></returns>
        public RewardStatus GetDateRewardStatus(int day) => this.uiTemplateDailyRewardData.RewardStatus[day - 1];

        public void ClaimAllAvailableReward()
        {
            var rewardsList = new List<Dictionary<string, int>>();
            for (var i = 0; i < this.uiTemplateDailyRewardData.RewardStatus.Count; i++)
            {
                if (this.uiTemplateDailyRewardData.RewardStatus[i] == RewardStatus.Unlocked)
                {
                    this.uiTemplateDailyRewardData.RewardStatus[i] = RewardStatus.Claimed;
                    rewardsList.Add(this.uiTemplateDailyRewardBlueprint.GetDataById(i + 1).Reward);
                }
            }

            var sumReward = rewardsList.SelectMany(d => d).GroupBy(kvp => kvp.Key, (key, kvps) => new { Key = key, Value = kvps.Sum(kvp => kvp.Value) }).ToDictionary(x => x.Key, x => x.Value);

            foreach (var reward in sumReward)
            {
                this.uiTemplateInventoryDataController.AddGenericReward(reward.Key, reward.Value);
            }
        }

        public async void InitRewardStatus()
        {
            this.uiTemplateDailyRewardData.RewardStatus = new();
            for (var i = 0; i < this.uiTemplateDailyRewardBlueprint.Values.Count; i++)
            {
                this.uiTemplateDailyRewardData.RewardStatus.Add(RewardStatus.Locked);
            }

            this.uiTemplateDailyRewardData.RewardStatus[0]  = RewardStatus.Unlocked;
            this.uiTemplateDailyRewardData.LastRewardedDate = await this.internetService.GetCurrentTimeAsync();
        }

        public bool CanClaimReward => this.uiTemplateDailyRewardData.RewardStatus.Any(t => t == RewardStatus.Unlocked);
    }
}