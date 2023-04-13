namespace TheOneStudio.UITemplate.UITemplate.Services.RewardHandle.AllRewards
{
    using System;
    using GameFoundation.Scripts.Utilities.LogService;
    using UnityEngine;

    public interface IUITemplateBaseReward
    {
        string        RewardId { get; }
        void          ReceiveReward(string value, string addressableFlyingItem);
        RectTransform StartPosAnimation { get; set; }
        RectTransform TargetPosAnimation { get; set; }
    }

    public abstract class UITemplateBaseReward : IUITemplateBaseReward
    {
        public abstract    string      RewardId { get; }
        protected readonly ILogService Logger;

        public RectTransform StartPosAnimation  { get; set; }
        public RectTransform TargetPosAnimation { get; set; }
        protected UITemplateBaseReward(ILogService logger) { this.Logger = logger; }

        public abstract void ReceiveReward(string value,string addressableFlyingItem);

        protected virtual T GetRewardValue<T>(string value) => (T)Convert.ChangeType(value, typeof(T));
    }
}