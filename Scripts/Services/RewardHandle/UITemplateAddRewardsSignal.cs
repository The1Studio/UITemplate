namespace TheOneStudio.UITemplate.UITemplate.Services.RewardHandle
{
    using System.Collections.Generic;
    using TheOneStudio.UITemplate.UITemplate.Models.LocalDatas;
    using UnityEngine;

    public class UITemplateAddRewardsSignal
    {
        public string                                       IapPackId        { get; }
        public Dictionary<string, UITemplateRewardItemData> RewardIdToData  { get; }
        public GameObject                                   SourceGameObject { get; }

        public UITemplateAddRewardsSignal(string iapPackId, Dictionary<string, UITemplateRewardItemData> rewardIdToData, GameObject sourceGameObject)
        {
            this.RewardIdToData  = rewardIdToData;
            this.SourceGameObject = sourceGameObject;
            this.IapPackId        = iapPackId;
        }
    }
}