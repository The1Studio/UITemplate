namespace TheOneStudio.UITemplate.Quests.Conditions
{
    using System;
    using GameFoundation.Signals;
    using Newtonsoft.Json;
    using TheOneStudio.UITemplate.UITemplate.Signals;
    using UnityEngine.Scripting;

    [Preserve]
    public sealed class CollectCurrencyCondition : BaseCondition
    {
        [JsonProperty] private string CurrencyId { get; [Preserve] set; }
        [JsonProperty] private int    Count      { get; [Preserve] set; }

        protected override ICondition.IProgress SetupProgress() => new Progress();

        [Preserve]
        private sealed class Progress : BaseProgress
        {
            [JsonProperty] private int Count { get; set; }

            protected override Type HandlerType => typeof(Handler);

            private sealed class Handler : BaseHandler<CollectCurrencyCondition, Progress>
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
                    this.signalBus.Subscribe<OnUpdateCurrencySignal>(this.OnUpdateCurrency);
                }

                private void OnUpdateCurrency(OnUpdateCurrencySignal @params)
                {
                    if (@params.Id != this.Condition.CurrencyId) return;
                    this.Progress.Count += @params.Amount;
                }

                protected override void Dispose()
                {
                    this.signalBus.Unsubscribe<OnUpdateCurrencySignal>(this.OnUpdateCurrency);
                }
            }
        }
    }
}