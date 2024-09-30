namespace TheOneStudio.UITemplate.Quests.Conditions
{
    using System;
    using Newtonsoft.Json;
    using UnityEngine.Scripting;

    [Preserve]
    public sealed class InTimeRangeCondition : BaseCondition
    {
        [JsonProperty] private DateTime Begin { get; [Preserve] set; }
        [JsonProperty] private DateTime End   { get; [Preserve] set; }

        protected override ICondition.IProgress SetupProgress() => new Progress();

        [Preserve]
        private sealed class Progress : BaseProgress
        {
            protected override Type HandlerType => typeof(Handler);

            [Preserve]
            private sealed class Handler : BaseHandler<InTimeRangeCondition, Progress>
            {
                protected override float CurrentProgress => DateTime.UtcNow > this.Condition.Begin && DateTime.UtcNow < this.Condition.End ? 1 : 0;

                protected override float MaxProgress => 1;
            }
        }
    }
}