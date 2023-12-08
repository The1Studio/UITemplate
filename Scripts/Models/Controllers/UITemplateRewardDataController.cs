namespace TheOneStudio.UITemplate.UITemplate.Models.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using GameFoundation.Scripts.Utilities.Extension;
    using TheOneStudio.UITemplate.UITemplate.Models.LocalDatas;

    public class UITemplateRewardDataController : IUITemplateControllerData
    {
        private readonly UITemplateRewardData uiTemplateRewardData;

        public UITemplateRewardDataController(UITemplateRewardData uiTemplateRewardData) { this.uiTemplateRewardData = uiTemplateRewardData; }

        public void AddRepeatedReward(string packID, Dictionary<string, UITemplateRewardItemData> rewardIdToData)
        {
            var storedRewardIdToData
                = this.uiTemplateRewardData.PackIdToIdToRewardData.GetOrAdd(packID, () => new Dictionary<string, UITemplateRewardItemData>());

            foreach (var (rewardId, rewardData) in rewardIdToData)
            {
                if (rewardData.Repeat == -1) continue;

                if (storedRewardIdToData.TryGetValue(rewardId, out var currentRewardData))
                {
                    currentRewardData.RewardValue += rewardData.RewardValue;
                }
                else
                {
                    rewardData.LastTimeReceive = DateTime.Now;
                    storedRewardIdToData.Add(rewardId, rewardData);
                }
            }
        }
        
        public Dictionary<string, int> GetAvailableRepeatedReward()
        {
            return this.uiTemplateRewardData.PackIdToIdToRewardData.Values
                .SelectMany(rewardIdToData => rewardIdToData.ToList())
                .Where(keyPairValue => keyPairValue.Value.LastTimeReceive.Day + keyPairValue.Value.Repeat >= DateTime.Now.Day)
                .GroupBy(keyPairValue => keyPairValue.Key)
                .ToDictionary(group => group.Key, group => group.Sum(keyPairValue => keyPairValue.Value.RewardValue));
        }
    }
}