﻿namespace TheOneStudio.HyperCasual.Quests.Conditions
{
    using System;
    using Newtonsoft.Json;
    using TheOneStudio.UITemplate.Quests.Conditions;
    using Zenject;

    public abstract class BaseSignalCountCondition<TSignal> : BaseCondition
    {
        [JsonProperty] private int Count { get; set; }

        protected override ICondition.IProgress SetupProgress() => new Progress();

        private sealed class Progress : BaseProgress
        {
            [JsonProperty] private int Count { get; set; }

            protected override Type HandlerType => typeof(Handler);

            private sealed class Handler : BaseHandler<BaseSignalCountCondition<TSignal>, Progress>
            {
                private readonly SignalBus signalBus;

                public Handler(SignalBus signalBus)
                {
                    this.signalBus = signalBus;
                }

                protected override float CurrentProgress => this.Progress.Count;
                protected override float MaxProgress     => this.Condition.Count;

                protected override void Initialize()
                {
                    this.signalBus.Subscribe<TSignal>(this.OnUpdate);
                }

                private void OnUpdate()
                {
                    ++this.Progress.Count;
                }

                protected override void Dispose()
                {
                    this.signalBus.Unsubscribe<TSignal>(this.OnUpdate);
                }
            }
        }
    }
}