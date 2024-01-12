namespace TheOneStudio.UITemplate.Quests.Rewards
{
    using Newtonsoft.Json;

    public abstract class BaseReward : IReward
    {
        [JsonProperty] public string Id    { get; private set; }
        [JsonProperty] public int    Value { get; private set; }
        [JsonProperty] public string Image { get; private set; }
    }
}