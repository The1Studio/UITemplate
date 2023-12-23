namespace TheOneStudio.UITemplate.UITemplate.Models.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using Cysharp.Threading.Tasks;
    using GameFoundation.Scripts.Utilities.Extension;
    using TheOneStudio.UITemplate.UITemplate.Blueprints;
    using TheOneStudio.UITemplate.UITemplate.Models.LocalDatas;
    using TheOneStudio.UITemplate.UITemplate.Scenes.Utils;
    using TheOneStudio.UITemplate.UITemplate.Services;
    using UnityEngine;

    public class UITemplateDailyRewardController : IUITemplateControllerData
    {
        private const int TotalDayInWeek = 7;

        #region inject

        private readonly IInternetService                    internetService;
        private readonly UITemplateDailyRewardData           uiTemplateDailyRewardData;
        private readonly UITemplateDailyRewardBlueprint      uiTemplateDailyRewardBlueprint;
        private readonly UITemplateInventoryDataController   uiTemplateInventoryDataController;
        private readonly UITemplateFlyingAnimationController uiTemplateFlyingAnimationController;

        #endregion

        private SemaphoreSlim mySemaphoreSlim = new(1, 1);

        public UITemplateDailyRewardController(IInternetService internetService, UITemplateDailyRewardData uiTemplateDailyRewardData, UITemplateDailyRewardBlueprint uiTemplateDailyRewardBlueprint,
            UITemplateInventoryDataController uiTemplateInventoryDataController, UITemplateFlyingAnimationController uiTemplateFlyingAnimationController)
        {
            this.internetService                     = internetService;
            this.uiTemplateDailyRewardData           = uiTemplateDailyRewardData;
            this.uiTemplateDailyRewardBlueprint      = uiTemplateDailyRewardBlueprint;
            this.uiTemplateInventoryDataController   = uiTemplateInventoryDataController;
            this.uiTemplateFlyingAnimationController = uiTemplateFlyingAnimationController;
        }

        public async UniTask CheckRewardStatus()
        {
            await this.mySemaphoreSlim.WaitAsync();

            try
            {
                // var currentTime = await this.internetService.GetCurrentTimeAsync();
                var currentTime = DateTime.Now; // Because the internet service getting time doesn't work stable I use this instead, btw, we allow hyper casual players cheat the game.
                var issDiffDay  = this.internetService.IsDifferentDay(this.uiTemplateDailyRewardData.LastRewardedDate, currentTime);

                if (!issDiffDay) return;

                var firstLockedDayIndex = this.FindFirstLockedDayIndex();

                if (firstLockedDayIndex == -1)
                {
                    if (!this.CanClaimReward)
                    {
                        this.InitRewardStatus(currentTime);
                    }
                }
                else
                {
                    if (firstLockedDayIndex / TotalDayInWeek == (firstLockedDayIndex) / TotalDayInWeek)
                    {
                        this.uiTemplateDailyRewardData.RewardStatus[firstLockedDayIndex] = RewardStatus.Unlocked;
                        this.uiTemplateDailyRewardData.LastRewardedDate                  = currentTime;
                    }
                }
            }
            finally
            {
                this.mySemaphoreSlim.Release();
            }
        }

        private int FindFirstLockedDayIndex() { return this.uiTemplateDailyRewardData.RewardStatus.FirstIndex(status => status is RewardStatus.Locked); }

        public int GetCurrentDayIndex()
        {
            var firstLockedDayIndex = this.FindFirstLockedDayIndex();

            return firstLockedDayIndex == -1 ? this.uiTemplateDailyRewardData.RewardStatus.Count - 1 : firstLockedDayIndex - 1;
        }

        /// <param name="day"> start from 1</param>
        /// <returns></returns>
        public RewardStatus GetDateRewardStatus(int day) => this.uiTemplateDailyRewardData.RewardStatus[day - 1];

        public async void ClaimAllAvailableReward(Dictionary<int, RectTransform> dailyIndexToRectTransform)
        {
            var rewardsList = new List<Dictionary<string, int>>();

            var playAnimTask = UniTask.CompletedTask;

            for (var i = 0; i < this.uiTemplateDailyRewardData.RewardStatus.Count; i++)
            {
                if (this.uiTemplateDailyRewardData.RewardStatus[i] == RewardStatus.Unlocked)
                {
                    this.uiTemplateDailyRewardData.RewardStatus[i] = RewardStatus.Claimed;

                    var reward = this.uiTemplateDailyRewardBlueprint.GetDataById(i + 1).Reward;

                    rewardsList.Add(reward);
                }
            }

            var sumReward = rewardsList.SelectMany(d => d).GroupBy(kvp => kvp.Key, (key, kvps) => new { Key = key, Value = kvps.Sum(kvp => kvp.Value) }).ToDictionary(x => x.Key, x => x.Value);

            await UniTask.WhenAny(playAnimTask);

            var rewardIndex = 0;
            foreach (var reward in sumReward)
            {
                this.uiTemplateInventoryDataController.AddGenericReward(reward.Key, reward.Value, dailyIndexToRectTransform[rewardIndex]);
                rewardIndex += 1;
            }
        }

        private void InitRewardStatus(DateTime currentTime)
        {
            this.uiTemplateDailyRewardData.RewardStatus = new();

            for (var i = 0; i < this.uiTemplateDailyRewardBlueprint.Values.Count; i++)
            {
                this.uiTemplateDailyRewardData.RewardStatus.Add(RewardStatus.Locked);
            }

            this.uiTemplateDailyRewardData.RewardStatus[0]  = RewardStatus.Unlocked;
            this.uiTemplateDailyRewardData.LastRewardedDate = currentTime;
        }

        public bool CanClaimReward => this.uiTemplateDailyRewardData.RewardStatus.Any(t => t == RewardStatus.Unlocked);

        public DateTime GetFirstTimeOpenedDate => this.uiTemplateDailyRewardData.FirstTimeOpenedDate;
    }
}