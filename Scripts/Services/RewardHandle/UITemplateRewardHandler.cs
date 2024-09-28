namespace TheOneStudio.UITemplate.UITemplate.Services.RewardHandle
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Cysharp.Threading.Tasks;
    using TheOneStudio.UITemplate.UITemplate.Models.Controllers;
    using TheOneStudio.UITemplate.UITemplate.Models.LocalDatas;
    using TheOneStudio.UITemplate.UITemplate.Services.RewardHandle.AllRewards;
    using UnityEngine;
    using UnityEngine.Scripting;

    public class UITemplateRewardHandler
    {
        private readonly IReadOnlyDictionary<string, IUITemplateRewardExecutor> rewardIdToRewardExecutor;
        private readonly UITemplateRewardDataController                         uiTemplateRewardDataController;
        private readonly UITemplateInventoryDataController                      uiTemplateInventoryDataController;

        [Preserve]
        public UITemplateRewardHandler(
            IEnumerable<IUITemplateRewardExecutor> rewardExecutors,
            UITemplateRewardDataController         uiTemplateRewardDataController,
            UITemplateInventoryDataController      uiTemplateInventoryDataController
        )
        {
            this.rewardIdToRewardExecutor          = rewardExecutors.ToDictionary(rewardExecutor => rewardExecutor.RewardId);
            this.uiTemplateRewardDataController    = uiTemplateRewardDataController;
            this.uiTemplateInventoryDataController = uiTemplateInventoryDataController;
        }

        public Dictionary<string, int> ClaimRepeatedReward()
        {
            var rewardList = this.uiTemplateRewardDataController.GetAvailableRepeatedReward();
            var availableRepeatedReward = rewardList
                .GroupBy(keyPairValue => keyPairValue.Key)
                .ToDictionary(group => group.Key, group => group.Sum(keyPairValue => keyPairValue.Value.RewardValue));

            foreach (var (rewardId, value) in availableRepeatedReward)
            {
                this.ReceiveReward(rewardId, value);
            }

            rewardList.ForEach(keyPairValue => keyPairValue.Value.LastTimeReceive = DateTime.Now);

            return availableRepeatedReward;
        }

        public void AddRewardsWithPackId(string iapPackId, Dictionary<string, UITemplateRewardItemData> rewardIdToData, GameObject sourceGameObject)
        {
            this.uiTemplateRewardDataController.AddRepeatedReward(iapPackId, rewardIdToData);

            this.AddRewards(rewardIdToData, sourceGameObject);
        }

        public void AddRewards(Dictionary<string, UITemplateRewardItemData> rewardIdToData, GameObject sourceGameObject)
        {
            foreach (var rewardData in rewardIdToData)
            {
                this.ReceiveReward(rewardData.Key, rewardData.Value.RewardValue, sourceGameObject == null ? null : sourceGameObject.transform as RectTransform);
            }
        }

        private void ReceiveReward(string rewardId, int rewardValue, RectTransform startPos = null)
        {
            if (this.rewardIdToRewardExecutor.TryGetValue(rewardId, out var dicRewardRecord))
            {
                dicRewardRecord.ReceiveReward(rewardValue, startPos);
            }
            else
            {
                this.uiTemplateInventoryDataController.AddGenericReward(rewardId, rewardValue, startPos).Forget();
            }
        }
    }
}