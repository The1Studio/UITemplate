namespace TheOneStudio.UITemplate.Quests.Conditions
{
    using System;
    using Newtonsoft.Json;
    using UnityEngine.Scripting;

    [Preserve]
    public sealed class AfterTimeCondition : BaseCondition
    {
        [JsonProperty] private DateTime Begin { get; [Preserve] set; }

        protected override ICondition.IProgress SetupProgress() => new Progress();

        [Preserve]
        private sealed class Progress : BaseProgress
        {
            protected override Type HandlerType => typeof(Handler);

            [Preserve]
            private sealed class Handler : BaseHandler<AfterTimeCondition, Progress>
            {
                protected override float CurrentProgress => DateTime.UtcNow > this.Condition.Begin ? 1 : 0;
                protected override float MaxProgress     => 1;
            }
        }
    }
}