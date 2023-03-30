namespace TheOneStudio.UITemplate.UITemplate.Models.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Cysharp.Threading.Tasks;
    using TheOneStudio.UITemplate.UITemplate.Blueprints;
    using TheOneStudio.UITemplate.UITemplate.Services;

    public class UITemplateDailyRewardController
    {
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

        public async UniTask<int> GetUserLoginDay()
        {
            var currentDay      = (await this.internetService.GetCurrentTimeAsync()).Day;
            var beginDay        = this.uiTemplateDailyRewardData.BeginDate.Day;
            var differenceOfDay = currentDay - beginDay;

            return differenceOfDay > 0 ? differenceOfDay >= this.uiTemplateDailyRewardData.RewardStatus.Count ? this.uiTemplateDailyRewardData.RewardStatus.Count - 1 : differenceOfDay : 0;
        }

        public void SetRewardStatus(int day, RewardStatus status)
        {
            if (this.uiTemplateDailyRewardData.RewardStatus[day] == RewardStatus.Claimed)
                return;

            this.uiTemplateDailyRewardData.BeginDate         = status == RewardStatus.Unlocked ? DateTime.Now : this.uiTemplateDailyRewardData.BeginDate;
            this.uiTemplateDailyRewardData.RewardStatus[day] = status;
        }

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
    }
}