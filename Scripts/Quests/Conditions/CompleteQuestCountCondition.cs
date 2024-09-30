namespace TheOneStudio.UITemplate.Quests.Conditions
{
    using System;
    using GameFoundation.Signals;
    using Newtonsoft.Json;
    using TheOneStudio.UITemplate.Quests.Data;
    using TheOneStudio.UITemplate.Quests.Signals;
    using UnityEngine.Scripting;

    [Preserve]
    public sealed class CompleteQuestCountCondition : BaseCondition
    {
        [JsonProperty] private string Tag   { get; [Preserve] set; }
        [JsonProperty] private int    Count { get; [Preserve] set; }

        protected override ICondition.IProgress SetupProgress() => new Progress();

        [Preserve]
        private sealed class Progress : BaseProgress
        {
            [JsonProperty] private int Count { get; set; }

            protected override Type HandlerType => typeof(Handler);

            private sealed class Handler : BaseHandler<CompleteQuestCountCondition, Progress>
            {
                private readonly SignalBus signalBus;

                [Preserve]
                public Handler(SignalBus signalBus)
                {
                    this.signalBus = signalBus;
                }

                protected override float CurrentProgress => this.Progress.Count;
                protected override float MaxProgress     => this.Condition.Count;

                protected override void Initialize()
                {
                    this.signalBus.Subscribe<QuestStatusChangedSignal>(this.QuestStatusChanged);
                }

                private void QuestStatusChanged(QuestStatusChangedSignal @params)
                {
                    if (@params.QuestController.Progress.Status is not QuestStatus.NotCollected) return;
                    if (this.Condition.Tag is { } tag && !@params.QuestController.Record.Tags.Contains(tag)) return;
                    ++this.Progress.Count;
                }

                protected override void Dispose()
                {
                    this.signalBus.Unsubscribe<QuestStatusChangedSignal>(this.QuestStatusChanged);
                }
            }
        }
    }
}