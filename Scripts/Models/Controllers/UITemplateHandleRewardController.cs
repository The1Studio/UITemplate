namespace TheOneStudio.UITemplate.UITemplate.Models.Controllers
{
    using System.Collections.Generic;
    using GameFoundation.Scripts.Utilities.Extension;
    using TheOneStudio.UITemplate.UITemplate.Models.LocalDatas;

    public class UITemplateHandleRewardController : IUITemplateControllerData
    {
        private readonly UITemplateRewardData uiTemplateRewardData;

        public UITemplateHandleRewardController(UITemplateRewardData uiTemplateRewardData) { this.uiTemplateRewardData = uiTemplateRewardData; }

        public void AddRepeatedReward(string packID, Dictionary<string, UITemplateRewardItemData> rewardIdToData)
        {
            var currentRewardIdToData = this.uiTemplateRewardData.IapPackIdToRewards.GetOrAdd(packID, () => new Dictionary<string, UITemplateRewardItemData>());

            foreach (var (rewardId, rewardData) in rewardIdToData)
            {
                if (rewardData.Repeat == -1) continue;

                if (currentRewardIdToData.TryGetValue(rewardId, out var currentRewardData))
                {
                    currentRewardData.RewardValue += rewardData.RewardValue;
                }
                else
                {
                    currentRewardIdToData.Add(rewardId, rewardData);
                }
            }
        }
    }
}