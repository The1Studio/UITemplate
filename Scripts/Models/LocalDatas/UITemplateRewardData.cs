namespace TheOneStudio.UITemplate.UITemplate.Models.LocalDatas
{
    using System;
    using System.Collections.Generic;
    using GameFoundation.Scripts.Interfaces;
    using Sirenix.Serialization;
    using TheOneStudio.UITemplate.UITemplate.Models.Controllers;
    using UnityEngine.Scripting;

    [Preserve]
    public class UITemplateRewardData : ILocalData, IUITemplateLocalData
    {
        //PackId can be any thing you want, it's just a key to store reward data
        [OdinSerialize]
        public Dictionary<string, Dictionary<string, UITemplateRewardItemData>> PackIdToIdToRewardData { get; set; } = new();

        public void Init()         { }
        public Type ControllerType => typeof(UITemplateRewardDataController);
    }

    [Serializable]
    public class UITemplateRewardItemData
    {
        [OdinSerialize]
        public int      RewardValue           { get; set; }
        [OdinSerialize]
        public int      Repeat                { get; set; }
        [OdinSerialize]
        public string   AddressableFlyingItem { get; set; }
        [OdinSerialize]
        public DateTime LastTimeReceive       { get; set; }

        public UITemplateRewardItemData(int rewardValue, int repeat, string addressableFlyingItem)
        {
            this.AddressableFlyingItem = addressableFlyingItem;
            this.RewardValue           = rewardValue;
            this.Repeat                = repeat;
        }
    }
}