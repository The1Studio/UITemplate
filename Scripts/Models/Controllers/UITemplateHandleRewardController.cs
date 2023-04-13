namespace TheOneStudio.UITemplate.UITemplate.Models.Controllers
{
    using System;
    using System.Collections.Generic;
    using TheOneStudio.UITemplate.UITemplate.Models.LocalDatas;
    using TheOneStudio.UITemplate.UITemplate.Services;

    public class UITemplateHandleRewardController
    {
        private readonly UITemplateRewardData uiTemplateRewardData;
        private readonly InternetService      internetService;

        public UITemplateHandleRewardController(UITemplateRewardData uiTemplateRewardData, InternetService internetService)
        {
            this.uiTemplateRewardData = uiTemplateRewardData;
            this.internetService      = internetService;
        }

        public void CheckToAddReward(string reward, int repeat, string value, string addressableFlyingItem)
        {
            if (!this.uiTemplateRewardData.Rewards.ContainsKey(reward))
            {
                this.uiTemplateRewardData.Rewards.Add(reward, new UITemplateRewardItemData(value, repeat, addressableFlyingItem));
            }
            else
            {
                if (this.uiTemplateRewardData.Rewards[reward].Repeat != -1)
                    this.uiTemplateRewardData.Rewards[reward].Repeat += repeat;
            }
        }

        public void CheckToRemoveReward(string reward)
        {
            if (this.uiTemplateRewardData.Rewards.TryGetValue(reward, out var itemData))
            {
                itemData.LastTimeReceive = DateTime.Now;
                if (itemData.Repeat == -1) return;
                itemData.Repeat--;
                if (itemData.Repeat == 0)
                {
                    this.uiTemplateRewardData.Rewards.Remove(reward);
                }
            }
        }

        public void ClearRewards() { this.uiTemplateRewardData.Rewards.Clear(); }

        public bool IsRewardExist(string reward) { return this.uiTemplateRewardData.Rewards.ContainsKey(reward); }

        public Dictionary<string, UITemplateRewardItemData> GetAllRewardCanReceiveAtThisTimeToDay()
        {
            var data = new Dictionary<string, UITemplateRewardItemData>();

            foreach (var record in this.uiTemplateRewardData.Rewards)
            {
                if (record.Value.Repeat is -1 or > 0 && this.internetService.IsDifferentDay(record.Value.LastTimeReceive, DateTime.Now))
                {
                    data.Add(record.Key, record.Value);
                }
            }

            return data;
        }

        public UITemplateRewardData CurrentReward => this.uiTemplateRewardData;
    }
}