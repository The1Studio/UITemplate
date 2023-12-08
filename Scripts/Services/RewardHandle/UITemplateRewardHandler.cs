namespace TheOneStudio.UITemplate.UITemplate.Services.RewardHandle
{
    using System.Collections.Generic;
    using TheOneStudio.UITemplate.UITemplate.Models.Controllers;
    using TheOneStudio.UITemplate.UITemplate.Models.LocalDatas;
    using TheOneStudio.UITemplate.UITemplate.Services.RewardHandle.AllRewards;
    using UnityEngine;
    using Zenject;

    public class UITemplateRewardHandler : IInitializable
    {
        #region inject

        private readonly List<IUITemplateRewardExecutor>   listRewardHandle;
        private readonly UITemplateRewardDataController    uiTemplateRewardDataController;
        private readonly UITemplateInventoryDataController uiTemplateInventoryDataController;

        #endregion

        private Dictionary<string, IUITemplateRewardExecutor> rewardIdToRewardExecutor = new();

        public UITemplateRewardHandler(List<IUITemplateRewardExecutor> listRewardHandle, UITemplateRewardDataController uiTemplateRewardDataController,
            UITemplateInventoryDataController uiTemplateInventoryDataController)
        {
            this.listRewardHandle                  = listRewardHandle;
            this.uiTemplateRewardDataController    = uiTemplateRewardDataController;
            this.uiTemplateInventoryDataController = uiTemplateInventoryDataController;
        }

        public void Initialize()
        {
            foreach (var data in this.listRewardHandle)
            {
                this.rewardIdToRewardExecutor.Add(data.RewardId, data);
            }
        }

        public Dictionary<string, int> ClaimRepeatedReward()
        {
            var availableRepeatedReward = this.uiTemplateRewardDataController.GetAvailableRepeatedReward();
            
            foreach (var (rewardId, value) in availableRepeatedReward)
            {
                this.ReceiveReward(rewardId, value);
            }

            return availableRepeatedReward;
        }

        public void AddRewardsWithPackId(string iapPackId, Dictionary<string, UITemplateRewardItemData> rewardIdToData, GameObject sourceGameObject)
        {
            this.uiTemplateRewardDataController.AddRepeatedReward(iapPackId, rewardIdToData);

            this.AdRewards(rewardIdToData, sourceGameObject);
        }

        public void AdRewards(Dictionary<string, UITemplateRewardItemData> rewardIdToData, GameObject sourceGameObject)
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
                this.uiTemplateInventoryDataController.AddGenericReward(rewardId, rewardValue);
            }
        }
    }
}