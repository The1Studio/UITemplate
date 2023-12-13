namespace TheOneStudio.UITemplate.Quests.Data.Rewards
{
    using System;

    public interface IRewardHandler
    {
        public Type RewardType { get; }

        public void Handle(IReward reward);
    }
}