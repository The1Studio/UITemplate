namespace TheOneStudio.UITemplate.Quests.Conditions
{
    using System;
    using Newtonsoft.Json;

    public sealed class NewMonthCondition : BaseCondition
    {
        protected override ICondition.IProgress SetupProgress() => new Progress();

        private sealed class Progress : BaseProgress
        {
            [JsonProperty] private DateTime? StartTime { get; set; }

            protected override Type HandlerType => typeof(Handler);

            private sealed class Handler : BaseHandler<NewMonthCondition, Progress>
            {
                protected override float CurrentProgress
                {
                    get
                    {
                        var now   = DateTime.UtcNow;
                        var start = this.Progress.StartTime.Value;
                        return now.Year > start.Year || now.Month > start.Month ? 1f : 0f;
                    }
                }

                protected override float MaxProgress => 1f;

                protected override void Initialize()
                {
                    this.Progress.StartTime ??= DateTime.UtcNow;
                }
            }
        }
    }
}