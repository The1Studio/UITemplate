namespace TheOneStudio.UITemplate.Quests.Data.Rewards
{
    using Newtonsoft.Json;

    public interface IReward
    {
        [JsonProperty] public string Id { get; }

        [JsonProperty] public int Value { get; }

        [JsonProperty] public string Image { get; }
    }
}