namespace TheOneStudio.UITemplate.UITemplate.Models.LocalDatas
{
    using System;
    using System.Collections.Generic;
    using GameFoundation.Scripts.Interfaces;

    public class UITemplateRewardData : ILocalData
    {
        public Dictionary<string, Dictionary<string, UITemplateRewardItemData>> IapPackIdToRewards { get; set; } = new();

        public void Init() { }
    }

    public class UITemplateRewardItemData
    {
        public string   RewardValue           { get; }
        public int      Repeat                { get; set; }
        public string   AddressableFlyingItem { get; }
        public int      TotalDayLeft          { get; set; }
        public DateTime LastTimeReceive       { get; set; }

        public UITemplateRewardItemData(string rewardValue, int repeat, string addressableFlyingItem, int totalDayLeft = 0)
        {
            this.AddressableFlyingItem = addressableFlyingItem;
            this.RewardValue           = rewardValue;
            this.Repeat                = repeat;
            this.TotalDayLeft          = totalDayLeft;
        }
    }
}