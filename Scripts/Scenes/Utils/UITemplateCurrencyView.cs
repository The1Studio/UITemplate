namespace TheOneStudio.UITemplate.UITemplate.Scenes.Utils
{
    using DG.Tweening;
    using GameFoundation.DI;
    using GameFoundation.Signals;
    using TheOneStudio.UITemplate.UITemplate.Models.Controllers;
    using TheOneStudio.UITemplate.UITemplate.Signals;
    using TMPro;
    using UnityEngine;

    public class UITemplateCurrencyView : UITemplateFlyingAnimationView
    {
        #region inject

        private SignalBus                         signalBus;
        private UITemplateInventoryDataController uiTemplateInventoryDataController;

        #endregion

        [SerializeField] public TMP_Text currencyValueText;

        [SerializeField] public string currencyId;

        [SerializeField] private float animDuration = 1.5f;

        public GameObject CurrencyIcon;

        private Tween updateCurrencyTween;

        private Color defaultColor = Color.white;

        protected virtual void Awake()
        {
            var container = this.GetCurrentContainer();
            this.signalBus                         = container.Resolve<SignalBus>();
            this.uiTemplateInventoryDataController = container.Resolve<UITemplateInventoryDataController>();

            this.Initialize();
            this.ResetState();
        }

        private void Initialize()
        {
            this.signalBus.Subscribe<OnFinishCurrencyAnimationSignal>(this.OnUpdateCurrency);
            this.UpdateData(this.uiTemplateInventoryDataController.GetCurrencyValue(this.currencyId));
            if (this.currencyValueText != null)
            {
                this.defaultColor = this.currencyValueText.color;
            }
        }

        private void ResetState()
        {
            this.CurrencyIcon.transform.localScale = Vector3.one;
            this.updateCurrencyTween?.Kill();
            this.currencyValueText.color = this.defaultColor;
        }

        private void UpdateData(int newValue) { this.currencyValueText.text = newValue.ToString(); }

        private void OnDestroy()
        {
            this.signalBus?.Unsubscribe<OnFinishCurrencyAnimationSignal>(this.OnUpdateCurrency);
        }

        private void OnUpdateCurrency(OnFinishCurrencyAnimationSignal obj)
        {
            if (!this.currencyId.Equals(obj.Id)) return;

            var yoyoTime   = 4;
            var scaleValue = 1.2f;
            this.CurrencyIcon.transform.DOScale(Vector3.one * scaleValue, this.animDuration / yoyoTime).SetLoops(yoyoTime, LoopType.Yoyo).SetUpdate(isIndependentUpdate: true);
            this.updateCurrencyTween?.Kill();
            this.currencyValueText.color = obj.Amount >= 0 ? Color.green : Color.red;

            this.updateCurrencyTween = DOTween.To(() => obj.FinalValue - obj.Amount, this.UpdateData, obj.FinalValue, this.animDuration).OnComplete(() =>
            {
                this.UpdateData(obj.FinalValue);
                this.ResetState();
            }).SetUpdate(isIndependentUpdate: true);
        }

        public override string CurrencyKey => this.currencyId;
    }
}