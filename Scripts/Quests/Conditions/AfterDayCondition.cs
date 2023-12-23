namespace TheOneStudio.UITemplate.Quests.Conditions
{
    using System;
    using Newtonsoft.Json;

    public sealed class AfterTimeCondition : BaseCondition
    {
        [JsonProperty] private DateTime Begin { get; set; }

        protected override ICondition.IProgress SetupProgress() => new Progress();

        private sealed class Progress : BaseProgress
        {
            protected override Type HandlerType => typeof(Handler);

            private sealed class Handler : BaseHandler<AfterTimeCondition, Progress>
            {
                protected override float CurrentProgress => DateTime.UtcNow > this.Condition.Begin ? 1 : 0;
                protected override float MaxProgress     => 1;
            }
        }
    }
}