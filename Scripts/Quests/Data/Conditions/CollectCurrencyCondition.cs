namespace TheOneStudio.UITemplate.Quests.Data.Conditions
{
    using System;
    using Newtonsoft.Json;
    using TheOneStudio.UITemplate.UITemplate.Signals;
    using UnityEngine;
    using Zenject;

    public class CollectCurrencyCondition : ICondition
    {
        [JsonProperty] [field: SerializeField] private string CurrencyId { get; set; }
        [JsonProperty] [field: SerializeField] private int    Count      { get; set; }

        ICondition.IProgress ICondition.SetupProgress() => new Progress();

        private class Progress : ICondition.IProgress
        {
            [JsonProperty] private int Count { get; set; }

            Type ICondition.IProgress.HandlerType => typeof(Handler);

            private class Handler : ConditionProgressHandler<CollectCurrencyCondition, Progress>
            {
                private readonly SignalBus signalBus;

                public Handler(SignalBus signalBus)
                {
                    this.signalBus = signalBus;
                }

                public override float CurrentProgress => this.Progress.Count;
                public override float MaxProgress     => this.Condition.Count;

                public override void Initialize()
                {
                    if (this.IsSatisfied()) return;
                    this.signalBus.Subscribe<UpdateCurrencySignal>(this.OnUpdateCurrency);
                }

                private void OnUpdateCurrency(UpdateCurrencySignal @params)
                {
                    if (@params.Id != this.Condition.CurrencyId) return;
                    this.Progress.Count += @params.Amount;
                    if (!this.IsSatisfied()) return;
                    this.Progress.Count = this.Condition.Count;
                    this.signalBus.Unsubscribe<UpdateCurrencySignal>(this.OnUpdateCurrency);
                }

                public override void Dispose()
                {
                    this.signalBus.TryUnsubscribe<UpdateCurrencySignal>(this.OnUpdateCurrency);
                }
            }
        }
    }
}