namespace TheOneStudio.UITemplate.Quests.Rewards
{
    using Newtonsoft.Json;
    using UnityEngine.Scripting;

    public abstract class BaseReward : IReward
    {
        [JsonProperty] public string Id    { get; [Preserve] private set; }
        [JsonProperty] public int    Value { get; [Preserve] private set; }
        [JsonProperty] public string Image { get; [Preserve] private set; }
    }
}