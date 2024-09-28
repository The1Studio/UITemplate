namespace TheOneStudio.UITemplate.UITemplate.Models.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using GameFoundation.Scripts.Utilities.Extension;
    using TheOneStudio.UITemplate.UITemplate.Models.LocalDatas;
    using UnityEngine.Scripting;

    public class UITemplateRewardDataController : IUITemplateControllerData
    {
        private readonly UITemplateRewardData uiTemplateRewardData;

        [Preserve]
        public UITemplateRewardDataController(UITemplateRewardData uiTemplateRewardData) { this.uiTemplateRewardData = uiTemplateRewardData; }

        public void AddRepeatedReward(string packID, Dictionary<string, UITemplateRewardItemData> rewardIdToData)
        {
            var storedRewardIdToData
                = this.uiTemplateRewardData.PackIdToIdToRewardData.GetOrAdd(packID, () => new Dictionary<string, UITemplateRewardItemData>());

            foreach (var (rewardId, rewardData) in rewardIdToData)
            {
                if (rewardData.Repeat <= 0) continue;

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

        public List<KeyValuePair<string, UITemplateRewardItemData>> GetAvailableRepeatedReward()
        {
            return this.uiTemplateRewardData.PackIdToIdToRewardData.Values
                .SelectMany(rewardIdToData => rewardIdToData.ToList())
                .Where(keyPairValue => keyPairValue.Value.LastTimeReceive.DayOfYear + keyPairValue.Value.Repeat <= DateTime.Now.DayOfYear)
                .ToList();
        }

        public bool IsExistAvailableRepeatedReward()
        {
            return this.GetAvailableRepeatedReward().Count > 0;
        }
    }
}