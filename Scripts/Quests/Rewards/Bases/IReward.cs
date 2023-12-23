namespace TheOneStudio.UITemplate.Quests.Rewards
{
    using System;

    public interface IReward
    {
        public string Id { get; }

        public int Value { get; }

        public string Image { get; }

        public interface IHandler
        {
            public Type RewardType { get; }

            public void Handle(IReward reward);
        }
    }
}