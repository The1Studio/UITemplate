namespace TheOneStudio.UITemplate.UITemplate.Services.RewardHandle
{
    using System.Collections.Generic;
    using TheOneStudio.UITemplate.UITemplate.Models.Controllers;
    using TheOneStudio.UITemplate.UITemplate.Services.RewardHandle.AllRewards;
    using UnityEngine;
    using Zenject;

    public class UITemplateRewardHandler : IInitializable
    {
        #region inject

        private readonly List<IUITemplateBaseReward>       listRewardHandle;
        private readonly UITemplateHandleRewardController  uiTemplateHandleRewardController;
        private readonly SignalBus                         signalBus;
        private readonly UITemplateInventoryDataController uiTemplateInventoryDataController;

        #endregion

        private Dictionary<string, IUITemplateBaseReward> dicRewardHandle = new();

        public UITemplateRewardHandler(List<IUITemplateBaseReward> listRewardHandle, UITemplateHandleRewardController uiTemplateHandleRewardController, SignalBus signalBus,
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
                this.dicRewardHandle.Add(data.RewardId, data);
            }

            this.signalBus.Subscribe<UITemplateAddRewardsSignal>(this.OnAddAndOnReceiveRewardNow);
        }

        private void OnAddAndOnReceiveRewardNow(UITemplateAddRewardsSignal obj)
        {
            this.uiTemplateHandleRewardController.AddRepeatedReward(obj.IapPackId, obj.RewardIdToData);

            foreach (var data in obj.RewardIdToData)
            {
                this.ReceiveReward(data.Key, data.Value.RewardValue, obj.SourceGameObject == null ? null : obj.SourceGameObject.transform as RectTransform);
            }
        }

        private void ReceiveReward(string rewardKey, int rewardValue, RectTransform startPos = null)
        {
            if (this.dicRewardHandle.TryGetValue(rewardKey, out var dicRewardRecord))
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