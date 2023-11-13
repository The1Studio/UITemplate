namespace TheOneStudio.UITemplate.UITemplate.Services.RewardHandle.AllRewards
{
    using UnityEngine;

    public interface IUITemplateBaseReward
    {
        string RewardId { get; }
        void   ReceiveReward(int value, RectTransform startPosAnimation);
    }

    public abstract class UITemplateBaseReward : IUITemplateBaseReward
    {
        public abstract string RewardId { get; }

        public abstract void ReceiveReward(int value, RectTransform startPosAnimation);
    }
}