namespace TheOneStudio.UITemplate.Quests.Rewards
{
    using System;
    using Newtonsoft.Json;

    public abstract class BaseReward : IReward
    {
        [JsonProperty] public string Id    { get; private set; }
        [JsonProperty] public int    Value { get; private set; }
        [JsonProperty] public string Image { get; private set; }

        protected abstract class BaseHandler<TReward> : IReward.IHandler
        {
            Type IReward.IHandler.RewardType => typeof(TReward);

            void IReward.IHandler.Handle(IReward reward) => this.Handle((TReward)reward);

            protected abstract void Handle(TReward reward);
        }
    }
}