namespace TheOneStudio.UITemplate.UITemplate.Services.RewardHandle.AllRewards
{
    using System;
    using GameFoundation.Scripts.Utilities.LogService;
    using UnityEngine;

    public interface IUITemplateBaseReward
    {
        string RewardId { get; }
        void   ReceiveReward(string value, RectTransform startPosAnimation);
    }

    public abstract class UITemplateBaseReward : IUITemplateBaseReward
    {
        public abstract    string                        RewardId { get; }
        protected readonly ILogService                   Logger;
        private readonly   UITemplateGetRealRewardHelper uiTemplateGetRealRewardHelper;

        protected UITemplateBaseReward(ILogService logger, UITemplateGetRealRewardHelper uiTemplateGetRealRewardHelper)
        {
            this.Logger                        = logger;
            this.uiTemplateGetRealRewardHelper = uiTemplateGetRealRewardHelper;
        }

        public abstract void ReceiveReward(string value, RectTransform startPosAnimation);

        protected virtual T GetRewardValue<T>(string value) => (T)Convert.ChangeType(value, typeof(T));

        protected virtual void AfterReceiveReward(string finalItemId) { this.uiTemplateGetRealRewardHelper.RealRewardItemReceived.Add(finalItemId); }
    }
}