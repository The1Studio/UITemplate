namespace TheOneStudio.UITemplate.UITemplate.Models.LocalDatas
{
    using System;
    using System.Collections.Generic;
    using GameFoundation.Scripts.Interfaces;
    using Newtonsoft.Json;
    using Sirenix.Serialization;
    using TheOneStudio.UITemplate.UITemplate.Models.Controllers;
    using UnityEngine.Scripting;

    [Preserve]
    public class UITemplateRewardData : ILocalData, IUITemplateLocalData
    {
        //PackId can be any thing you want, it's just a key to store reward data
        [JsonProperty] [OdinSerialize] internal Dictionary<string, Dictionary<string, UITemplateRewardItemData>> PackIdToIdToRewardData { get; set; } = new();

        public void Init() { }

        public Type ControllerType => typeof(UITemplateRewardDataController);
    }

    [Serializable]
    public class UITemplateRewardItemData
    {
        [JsonProperty] [OdinSerialize] public int      RewardValue           { get; internal set; }
        [JsonProperty] [OdinSerialize] public int      Repeat                { get; internal set; }
        [JsonProperty] [OdinSerialize] public string   AddressableFlyingItem { get; internal set; }
        [JsonProperty] [OdinSerialize] public DateTime LastTimeReceive       { get; internal set; }

        public UITemplateRewardItemData(int rewardValue, int repeat, string addressableFlyingItem)
        {
            this.AddressableFlyingItem = addressableFlyingItem;
            this.RewardValue           = rewardValue;
            this.Repeat                = repeat;
        }
    }
}