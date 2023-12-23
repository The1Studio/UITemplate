namespace TheOneStudio.UITemplate.Quests.Conditions
{
    using System;
    using Newtonsoft.Json;

    public sealed class InTimeRangeCondition : BaseCondition
    {
        [JsonProperty] private DateTime Begin { get; set; }
        [JsonProperty] private DateTime End   { get; set; }

        protected override ICondition.IProgress SetupProgress() => new Progress();

        private sealed class Progress : BaseProgress
        {
            protected override Type HandlerType => typeof(Handler);

            private sealed class Handler : BaseHandler<InTimeRangeCondition, Progress>
            {
                protected override float CurrentProgress => DateTime.UtcNow > this.Condition.Begin && DateTime.UtcNow < this.Condition.End ? 1 : 0;

                protected override float MaxProgress => 1;
            }
        }
    }
}