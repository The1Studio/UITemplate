namespace TheOneStudio.UITemplate.Quests.Conditions
{
    using System;
    using System.Globalization;
    using Newtonsoft.Json;
    using UnityEngine.Scripting;

    [Preserve]
    public sealed class NewWeekCondition : BaseCondition
    {
        protected override ICondition.IProgress SetupProgress() => new Progress();

        [Preserve]
        private sealed class Progress : BaseProgress
        {
            [JsonProperty] private DateTime? StartTime { get; set; }

            protected override Type HandlerType => typeof(Handler);

            [Preserve]
            private sealed class Handler : BaseHandler<NewWeekCondition, Progress>
            {
                protected override float CurrentProgress
                {
                    get
                    {
                        var calendar = DateTimeFormatInfo.CurrentInfo.Calendar;
                        var now      = DateTime.UtcNow;
                        var start    = this.Progress.StartTime.Value;
                        now   = now.Date.AddDays(-1 * (int)calendar.GetDayOfWeek(now));
                        start = start.Date.AddDays(-1 * (int)calendar.GetDayOfWeek(start));
                        return now > start ? 1f : 0f;
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