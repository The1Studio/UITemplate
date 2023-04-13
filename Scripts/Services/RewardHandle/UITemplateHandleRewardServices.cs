namespace TheOneStudio.UITemplate.UITemplate.Services.RewardHandle
{
    using System;
    using System.Collections.Generic;
    using GameFoundation.Scripts.UIModule.ScreenFlow.Signals;
    using GameFoundation.Scripts.Utilities.LogService;
    using TheOneStudio.UITemplate.UITemplate.Models.Controllers;
    using TheOneStudio.UITemplate.UITemplate.Scenes.Main;
    using TheOneStudio.UITemplate.UITemplate.Services.RewardHandle.AllRewards;
    using UnityEngine;
    using Zenject;

    public class UITemplateHandleRewardServices : IInitializable
    {
        private readonly List<IUITemplateBaseReward>               listRewardHandle;
        private readonly ILogService                               logger;
        private readonly UITemplateHandleRewardController          uiTemplateHandleRewardController;
        private readonly SignalBus                                 signalBus;
        private          Dictionary<string, IUITemplateBaseReward> dicRewardHandle = new();

        public UITemplateHandleRewardServices(List<IUITemplateBaseReward> listRewardHandle, ILogService logger, UITemplateHandleRewardController uiTemplateHandleRewardController, SignalBus signalBus)
        {
            this.listRewardHandle                 = listRewardHandle;
            this.logger                           = logger;
            this.uiTemplateHandleRewardController = uiTemplateHandleRewardController;
            this.signalBus                        = signalBus;
        }

        public void Initialize()
        {
            foreach (var data in this.listRewardHandle)
            {
                this.dicRewardHandle.Add(data.RewardId, data);
            }

            this.signalBus.Subscribe<UITemplateAddRewardsSignal>(this.OnAddAndOnReceiveRewardNow);
            this.signalBus.Subscribe<ScreenShowSignal>(this.OnScreenShow);
        }

        private void OnScreenShow(ScreenShowSignal obj)
        {
            if (obj.ScreenPresenter is UITemplateHomeSimpleScreenPresenter or UITemplateHomeTapToPlayScreenPresenter)
                this.OnShowRewardToday(new Dictionary<string, Tuple<RectTransform, RectTransform>>());
        }

        public void OnShowRewardToday(Dictionary<string, Tuple<RectTransform, RectTransform>> startPosAnimationDictionary)
        {
            //Todo Call at specific screen
            var totalReward = this.uiTemplateHandleRewardController.GetAllRewardCanReceiveAtThisTimeToDay();

            if (totalReward.Count < 0) return;
            this.logger.Error($"Total reward: {totalReward.Count} need UI To display Total reward ToDay, remove this log when UI is ready");

            foreach (var data in totalReward)
            {
                //Set StartPosAnimation for each reward

                if (startPosAnimationDictionary.TryGetValue(data.Key, out var startPosAnimationRecord) && this.dicRewardHandle.ContainsKey(data.Key))
                {
                    this.dicRewardHandle[data.Key].StartPosAnimation  = startPosAnimationRecord.Item1;
                    this.dicRewardHandle[data.Key].TargetPosAnimation = startPosAnimationRecord.Item2;
                }

                this.ReceiveReward(data.Key, data.Value.RewardValue, data.Value.AddressableFlyingItem);
            }
        }

        private void OnAddAndOnReceiveRewardNow(UITemplateAddRewardsSignal obj)
        {
            foreach (var data in obj.RewardItemDatas)
            {
                this.uiTemplateHandleRewardController.CheckToAddReward(data.Key, data.Value.Repeat, data.Value.RewardValue, data.Value.AddressableFlyingItem);

                if (this.dicRewardHandle.TryGetValue(data.Key, out var dicRewardRecord))
                {
                    dicRewardRecord.ReceiveReward(data.Value.RewardValue, data.Value.AddressableFlyingItem);
                }

                this.uiTemplateHandleRewardController.CheckToRemoveReward(data.Key);
            }
        }

        private void ReceiveReward(string rewardKey, string value, string addressableFlyingItem)
        {
            if (!this.dicRewardHandle.TryGetValue(rewardKey, out var dicRewardRecord)) return;
            dicRewardRecord.ReceiveReward(value, addressableFlyingItem);
            this.uiTemplateHandleRewardController.CheckToRemoveReward(rewardKey);
        }
    }
}