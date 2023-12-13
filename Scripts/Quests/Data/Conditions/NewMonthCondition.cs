namespace TheOneStudio.UITemplate.Quests.Data.Conditions
{
    using System;
    using Newtonsoft.Json;

    public class NewMonthCondition : ICondition
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
                        var now   = DateTime.Now;
                        var start = this.Progress.StartTime.Value;
                        return now.Year > start.Year || now.Month > start.Month ? 1f : 0f;
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