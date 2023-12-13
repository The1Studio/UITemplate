namespace TheOneStudio.UITemplate.Quests.Data.Conditions
{
    using System;
    using Newtonsoft.Json;

    public class NewDayCondition : ICondition
    {
        ICondition.IProgress ICondition.SetupProgress() => new Progress();

        private class Progress : ICondition.IProgress
        {
            [JsonProperty] private DateTime? StartTime { get; set; }

            Type ICondition.IProgress.HandlerType => typeof(Handler);

            private class Handler : ConditionProgressHandler<NewDayCondition, Progress>
            {
                public override float CurrentProgress => DateTime.Now.Date > this.Progress.StartTime.Value.Date ? 1f : 0f;
                public override float MaxProgress     => 1f;

                public override void Initialize()
                {
                    this.Progress.StartTime ??= DateTime.Now;
                }
            }
        }
    }
}