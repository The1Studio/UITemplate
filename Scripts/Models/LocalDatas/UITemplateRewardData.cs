namespace TheOneStudio.UITemplate.UITemplate.Models.LocalDatas
{
    using System;
    using System.Collections.Generic;
    using GameFoundation.Scripts.Interfaces;
    using TheOneStudio.UITemplate.UITemplate.Models.Controllers;

    public class UITemplateRewardData : ILocalData, IUITemplateLocalData
    {
        public Dictionary<string, Dictionary<string, UITemplateRewardItemData>> PackIdToIdToRewardData { get; set; } = new();

        public void Init()         { }
        public Type ControllerType => typeof(UITemplateRewardDataController);
    }

    public class UITemplateRewardItemData
    {
        public int      RewardValue           { get; set; }
        public int      Repeat                { get; set; }
        public string   AddressableFlyingItem { get; set; }
        public DateTime LastTimeReceive       { get; set; }

        public UITemplateRewardItemData(int rewardValue, int repeat, string addressableFlyingItem)
        {
            this.AddressableFlyingItem = addressableFlyingItem;
            this.RewardValue           = rewardValue;
            this.Repeat                = repeat;
        }
    }
}