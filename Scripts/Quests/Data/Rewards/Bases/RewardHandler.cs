namespace TheOneStudio.UITemplate.Quests.Data.Rewards
{
    using System;

    public abstract class RewardHandler<TReward> : IRewardHandler
    {
        Type IRewardHandler.RewardType => typeof(TReward);

        void IRewardHandler.Handle(IReward reward) => this.Handle((TReward)reward);

        protected abstract void Handle(TReward reward);
    }
}