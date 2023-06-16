namespace TheOneStudio.UITemplate.UITemplate.Scenes.Utils
{
    using DG.Tweening;
    using TheOneStudio.UITemplate.UITemplate.Signals;
    using TMPro;
    using UnityEngine;
    using Zenject;

    public class UITemplateCurrencyView : UITemplateFlyingAnimationView
    {
        [SerializeField] private TMP_Text currencyValueText;

        [SerializeField] private string currencyId;

        public GameObject CurrencyIcon;

        private Tween updateCurrencyTween;

        private Color _defaultColor = Color.white;

        public void Subscribe(SignalBus signalBus, int initValue)
        {
            signalBus.Subscribe<UpdateCurrencySignal>(this.UpdateCurrency);
            this.UpdateData(initValue);
            if (this.currencyValueText != null && this.currencyValueText.color != null)
            {
                _defaultColor = this.currencyValueText.color;
            }

            this.ResetState();
        }

        private void ResetState()
        {
            this.CurrencyIcon.transform.localScale = Vector3.one;
            this.updateCurrencyTween?.Kill();
            this.currencyValueText.color = _defaultColor;
        }

        private void UpdateData(int newValue) { this.currencyValueText.text = newValue.ToString(); }

        public void Unsubscribe(SignalBus signalBus) { signalBus.Unsubscribe<UpdateCurrencySignal>(this.UpdateCurrency); }

        private void UpdateCurrency(UpdateCurrencySignal obj)
        {
            if (!this.currencyId.Equals(obj.Id)) return;
            var duration   = 1.5f;
            var yoyoTime   = 4;
            var scaleValue = 1.2f;
            this.CurrencyIcon.transform.DOScale(Vector3.one * scaleValue, duration / yoyoTime).SetLoops(yoyoTime, LoopType.Yoyo);
            this.updateCurrencyTween?.Kill();
            this.currencyValueText.color = obj.Amount >= 0 ? Color.green : Color.red;

            this.updateCurrencyTween = DOTween.To(() => obj.FinalValue - obj.Amount, this.UpdateData, obj.FinalValue, duration).OnComplete(() =>
            {
                this.UpdateData(obj.FinalValue);
                this.ResetState();
            });
        }
    }
}