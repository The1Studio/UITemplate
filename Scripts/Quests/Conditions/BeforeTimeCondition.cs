namespace TheOneStudio.UITemplate.Quests.Conditions
{
    using System;
    using Newtonsoft.Json;

    public sealed class BeforeTimeCondition : BaseCondition
    {
        [JsonProperty] private DateTime End { get; }

        protected override ICondition.IProgress SetupProgress() => new Progress();

        private sealed class Progress : BaseProgress
        {
            protected override Type HandlerType => typeof(Handler);

            private sealed class Handler : BaseHandler<BeforeTimeCondition, Progress>
            {
                protected override float CurrentProgress => DateTime.UtcNow < this.Condition.End ? 1 : 0;
                protected override float MaxProgress     => 1;
            }
        }
    }
}