namespace TheOneStudio.UITemplate.UITemplate.Models.LocalDatas
{
    using System;
    using System.Collections.Generic;
    using GameFoundation.Scripts.Interfaces;
    using TheOneStudio.UITemplate.UITemplate.Models.Controllers;

    public class UITemplateRewardData : ILocalData, IUITemplateLocalData
    {
        public Dictionary<string, Dictionary<string, UITemplateRewardItemData>> IapPackIdToRewards { get; set; } = new();

        public void Init()         { }
        public Type ControllerType => typeof(UITemplateHandleRewardController);
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