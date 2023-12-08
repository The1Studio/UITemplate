namespace TheOneStudio.UITemplate.UITemplate.Services.RewardHandle
{
    using System.Collections.Generic;
    using TheOneStudio.UITemplate.UITemplate.Models.Controllers;
    using TheOneStudio.UITemplate.UITemplate.Models.LocalDatas;
    using TheOneStudio.UITemplate.UITemplate.Services.RewardHandle.AllRewards;
    using TheOneStudio.UITemplate.UITemplate.Signals;
    using UnityEngine;
    using Zenject;

    public class UITemplateRewardHandler : IInitializable
    {
        #region inject

        private readonly SignalBus                         signalBus;
        private readonly List<IUITemplateRewardExecutor>       listRewardHandle;
        private readonly UITemplateHandleRewardController  uiTemplateHandleRewardController;
        private readonly UITemplateInventoryDataController uiTemplateInventoryDataController;

        #endregion

        private Dictionary<string, IUITemplateRewardExecutor> rewardIdToRewardExecutor = new();

        public UITemplateRewardHandler(List<IUITemplateRewardExecutor> listRewardHandle, UITemplateHandleRewardController uiTemplateHandleRewardController, SignalBus signalBus,
            UITemplateInventoryDataController uiTemplateInventoryDataController)
        {
            this.listRewardHandle                  = listRewardHandle;
            this.uiTemplateHandleRewardController  = uiTemplateHandleRewardController;
            this.signalBus                         = signalBus;
            this.uiTemplateInventoryDataController = uiTemplateInventoryDataController;
        }

        public void Initialize()
        {
            foreach (var data in this.listRewardHandle)
            {
                this.rewardIdToRewardExecutor.Add(data.RewardId, data);
            }
        }
        
        public void AddRewards(string iapPackId, Dictionary<string, UITemplateRewardItemData> rewardIdToData, GameObject sourceGameObject)
        {
            this.uiTemplateHandleRewardController.AddRepeatedReward(iapPackId, rewardIdToData);

            foreach (var data in rewardIdToData)
            {
                this.ReceiveReward(data.Key, data.Value.RewardValue, sourceGameObject == null ? null : sourceGameObject.transform as RectTransform);
            }
        }

        private void ReceiveReward(string rewardKey, int rewardValue, RectTransform startPos = null)
        {
            if (this.rewardIdToRewardExecutor.TryGetValue(rewardKey, out var dicRewardRecord))
            {
                dicRewardRecord.ReceiveReward(rewardValue, startPos);
            }
            else
            {
                this.uiTemplateInventoryDataController.AddGenericReward(rewardKey, rewardValue);
            }
        }
    }
}