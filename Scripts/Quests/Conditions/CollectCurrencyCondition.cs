namespace TheOneStudio.UITemplate.Quests.Conditions
{
    using System;
    using Newtonsoft.Json;
    using TheOneStudio.UITemplate.UITemplate.Signals;
    using Zenject;

    public sealed class CollectCurrencyCondition : BaseCondition
    {
        [JsonProperty] private string CurrencyId { get; set; }
        [JsonProperty] private int    Count      { get; set; }

        protected override ICondition.IProgress SetupProgress() => new Progress();

        private sealed class Progress : BaseProgress
        {
            [JsonProperty] private int Count { get; set; }

            protected override Type HandlerType => typeof(Handler);

            private sealed class Handler : BaseHandler<CollectCurrencyCondition, Progress>
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
                    this.signalBus.Subscribe<UpdateCurrencySignal>(this.OnUpdateCurrency);
                }

                private void OnUpdateCurrency(UpdateCurrencySignal @params)
                {
                    if (@params.Id != this.Condition.CurrencyId) return;
                    this.Progress.Count += @params.Amount;
                }

                protected override void Dispose()
                {
                    this.signalBus.Unsubscribe<UpdateCurrencySignal>(this.OnUpdateCurrency);
                }
            }
        }
    }
}