namespace TheOneStudio.UITemplate.Quests.Rewards
{
    public interface IReward
    {
        public string Id { get; }

        public int Value { get; }

        public string Image { get; }
    }
}