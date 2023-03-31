namespace TheOneStudio.UITemplate.UITemplate.Scenes.Utils
{
    using System;
    using Cysharp.Threading.Tasks;
    using DG.Tweening;
    using TheOneStudio.UITemplate.UITemplate.Signals;
    using TMPro;
    using UnityEngine;
    using Zenject;

    public class UITemplateCurrencyView : MonoBehaviour
    {
        [SerializeField] private TMP_Text currencyValueText;

        [SerializeField] private string currencyId;

        public GameObject CurrencyIcon;

        private Tween updateCurrencyTween;

        public void Subscribe(SignalBus signalBus, int initValue)
        {
            signalBus.Subscribe<UpdateCurrencySignal>(this.UpdateCurrency);
            this.UpdateData(initValue);
            this.ResetState();
        }

        private void ResetState()
        {
            this.CurrencyIcon.transform.localScale = Vector3.one;
            this.updateCurrencyTween?.Kill();
            this.currencyValueText.color = Color.white;
        }

        private void UpdateData(int newValue) { this.currencyValueText.text = newValue.ToString(); }

        public void Unsubscribe(SignalBus signalBus) { signalBus.TryUnsubscribe<UpdateCurrencySignal>(this.UpdateCurrency); }

        private async void UpdateCurrency(UpdateCurrencySignal obj)
        {
            if (!this.currencyId.Equals(obj.Id)) return;
            var duration   = 1.5f;
            var yoyoTime   = 4;
            var scaleValue = 1.2f;
            this.CurrencyIcon.transform.DOScale(Vector3.one * scaleValue, duration / yoyoTime).SetLoops(yoyoTime, LoopType.Yoyo);
            this.updateCurrencyTween?.Kill();
            this.updateCurrencyTween     = DOTween.To(() => obj.FinalValue - obj.Amount, this.UpdateData, obj.FinalValue, duration);
            this.currencyValueText.color = obj.Amount >= 0 ? Color.green : Color.red;
            await UniTask.Delay(TimeSpan.FromSeconds(duration));
            this.ResetState();
        }
    }
}