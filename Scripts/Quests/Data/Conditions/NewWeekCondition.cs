namespace TheOneStudio.UITemplate.Quests.Data.Conditions
{
    using System;
    using System.Globalization;
    using Newtonsoft.Json;

    public class NewWeekCondition : ICondition
    {
        ICondition.IProgress ICondition.SetupProgress() => new Progress();

        private class Progress : ICondition.IProgress
        {
            [JsonProperty] private DateTime? StartTime { get; set; }

            Type ICondition.IProgress.HandlerType => typeof(Handler);

            private class Handler : ConditionProgressHandler<NewDayCondition, Progress>
            {
                public override float CurrentProgress
                {
                    get
                    {
                        var calendar = DateTimeFormatInfo.CurrentInfo.Calendar;
                        var now      = DateTime.Now;
                        var start    = this.Progress.StartTime.Value;
                        now   = now.Date.AddDays(-1 * (int)calendar.GetDayOfWeek(now));
                        start = start.Date.AddDays(-1 * (int)calendar.GetDayOfWeek(start));
                        return now > start ? 1f : 0f;
                    }
                }

                public override float MaxProgress => 1f;

                public override void Initialize()
                {
                    this.Progress.StartTime ??= DateTime.Now;
                }
            }
        }
    }
}