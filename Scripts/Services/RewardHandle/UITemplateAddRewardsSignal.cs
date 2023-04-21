namespace TheOneStudio.UITemplate.UITemplate.Services.RewardHandle
{
    using System.Collections.Generic;
    using BlueprintFlow.Signals;
    using TheOneStudio.UITemplate.UITemplate.Models.LocalDatas;
    using TheOneStudio.UITemplate.UITemplate.Signals;
    using UnityEngine;

    public class UITemplateAddRewardsSignal:ISignal
    {
        public string                                       IapPackId        { get; }
        public Dictionary<string, UITemplateRewardItemData> RewardItemDatas  { get; }
        public GameObject                                   SourceGameObject { get; }

        public UITemplateAddRewardsSignal(string iapPackId, Dictionary<string, UITemplateRewardItemData> rewardItemDatas, GameObject sourceGameObject)
        {
            this.RewardItemDatas  = rewardItemDatas;
            this.SourceGameObject = sourceGameObject;
            this.IapPackId        = iapPackId;
        }
    }
}