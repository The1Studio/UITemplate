namespace TheOneStudio.UITemplate.Quests.Conditions
{
    using System;
    using Newtonsoft.Json;
    using UnityEngine.Scripting;

    [Preserve]
    public sealed class NewDayCondition : BaseCondition
    {
        protected override ICondition.IProgress SetupProgress() => new Progress();

        [Preserve]
        private sealed class Progress : BaseProgress
        {
            [JsonProperty] private DateTime? StartTime { get; set; }

            protected override Type HandlerType => typeof(Handler);

            [Preserve]
            private sealed class Handler : BaseHandler<NewDayCondition, Progress>
            {
                protected override float CurrentProgress => DateTime.UtcNow.Date > this.Progress.StartTime.Value.Date ? 1f : 0f;
                protected override float MaxProgress     => 1f;

                protected override void Initialize()
                {
                    this.Progress.StartTime ??= DateTime.UtcNow;
                }
            }
        }
    }
}