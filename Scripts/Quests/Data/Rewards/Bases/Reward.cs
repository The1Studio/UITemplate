namespace TheOneStudio.UITemplate.Quests.Data.Rewards
{
    using UnityEngine;

    public abstract class Reward : IReward
    {
        [field: SerializeField] public string Id    { get; private set; }
        [field: SerializeField] public int    Value { get; private set; }
        [field: SerializeField] public string Image { get; private set; }
    }
}