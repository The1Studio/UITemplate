namespace TheOneStudio.UITemplate.UITemplate.Services.RewardHandle.AllRewards
{
    using UnityEngine;

    public interface IUITemplateRewardExecutor
    {
        string RewardId { get; }
        void   ReceiveReward(int value, RectTransform startPosAnimation);
    }

    public abstract class UITemplateRewardExecutorBase : IUITemplateRewardExecutor
    {
        public abstract string RewardId { get; }

        public abstract void ReceiveReward(int value, RectTransform startPosAnimation);
    }
}