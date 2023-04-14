namespace TheOneStudio.UITemplate.UITemplate.Models.Controllers
{
    using System;
    using System.Collections.Generic;
    using GameFoundation.Scripts.Utilities.LogService;
    using TheOneStudio.UITemplate.UITemplate.Models.LocalDatas;
    using TheOneStudio.UITemplate.UITemplate.Services;
    using UnityEngine;

    public class UITemplateHandleRewardController
    {
        private readonly UITemplateRewardData uiTemplateRewardData;
        private readonly ILogService          logger;
        private readonly InternetService      internetService;

        public UITemplateHandleRewardController(UITemplateRewardData uiTemplateRewardData, ILogService logger, InternetService internetService)
        {
            this.uiTemplateRewardData = uiTemplateRewardData;
            this.logger               = logger;
            this.internetService      = internetService;
        }

        public void CheckToAddReward(string packID, Dictionary<string, UITemplateRewardItemData> rewardItemDatas)
        {
            if (!this.uiTemplateRewardData.IapPackIdToRewards.ContainsKey(packID))
            {
                this.uiTemplateRewardData.IapPackIdToRewards.Add(packID, rewardItemDatas);
            }
            else
            {
                var packIdToReward = this.uiTemplateRewardData.IapPackIdToRewards[packID];

                foreach (var rewardItemData in rewardItemDatas)
                {
                    if (packIdToReward.ContainsKey(rewardItemData.Key))
                    {
                        if (packIdToReward[rewardItemData.Key].Repeat != -1)
                        {
                            packIdToReward[rewardItemData.Key].Repeat = Math.Min(rewardItemData.Value.Repeat, packIdToReward[rewardItemData.Key].Repeat);
                        }
                    }
                    else
                    {
                        packIdToReward.Add(rewardItemData.Key, rewardItemData.Value);
                    }
                }
            }
        }

        public void CheckToRemoveReward(string packID, string rewardId)
        {
            if (this.uiTemplateRewardData.IapPackIdToRewards.TryGetValue(packID, out var itemData))
            {
                if (itemData.TryGetValue(rewardId, out var rewardItemData))
                {
                    rewardItemData.LastTimeReceive = DateTime.Now;

                    if (rewardItemData.Repeat == -1)
                    {
                        itemData.Remove(rewardId);

                        if (itemData.Count == 0)
                        {
                            this.uiTemplateRewardData.IapPackIdToRewards.Remove(packID);
                        }
                    }

                    //Todo Handle TotalDayLeft
                    this.logger.LogWithColor($"Handle total day left later", Color.green);
                    // rewardItemData.TotalDayLeft--;
                }
            }
        }

        public void ClearRewards() { this.uiTemplateRewardData.IapPackIdToRewards.Clear(); }

        public bool IsRewardExist(string packId, string rewardId)
        {
            return this.uiTemplateRewardData.IapPackIdToRewards.ContainsKey(packId) && this.uiTemplateRewardData.IapPackIdToRewards[packId].ContainsKey(rewardId);
        }

        public Dictionary<string, Dictionary<string, UITemplateRewardItemData>> GetAllRewardCanReceiveAtThisTimeToDay()
        {
            var data = new Dictionary<string, Dictionary<string, UITemplateRewardItemData>>();

            foreach (var iapPackIdToReward in this.uiTemplateRewardData.IapPackIdToRewards)
            {
                data.Add(iapPackIdToReward.Key, new Dictionary<string, UITemplateRewardItemData>());

                foreach (var rewardItemData in iapPackIdToReward.Value)
                {
                    var totalDiffDay = this.internetService.ToTalDiffDay(rewardItemData.Value.LastTimeReceive, DateTime.Now);

                    if (totalDiffDay >= rewardItemData.Value.Repeat && rewardItemData.Value.TotalDayLeft >= 0)
                    {
                        data[iapPackIdToReward.Key].Add(rewardItemData.Key, rewardItemData.Value);
                    }
                }

                if (data[iapPackIdToReward.Key].Count == 0)
                {
                    data.Remove(iapPackIdToReward.Key);
                }
            }

            return data;
        }

        public UITemplateRewardData CurrentReward => this.uiTemplateRewardData;
    }
}