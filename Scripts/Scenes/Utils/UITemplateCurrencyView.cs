namespace TheOneStudio.UITemplate.UITemplate.Scenes.Utils
{
    using System;
    using DG.Tweening;
    using TheOneStudio.UITemplate.UITemplate.Models.Controllers;
    using TheOneStudio.UITemplate.UITemplate.Signals;
    using TMPro;
    using UnityEngine;
    using Zenject;

    public class UITemplateCurrencyView : UITemplateFlyingAnimationView
    {
        #region inject

        private SignalBus                         signalBus;
        private UITemplateInventoryDataController uiTemplateInventoryDataController;

        #endregion
        
        [SerializeField] private TMP_Text currencyValueText;

        [SerializeField] private string currencyId;

        public GameObject CurrencyIcon;

        private Tween updateCurrencyTween;

        private Color defaultColor = Color.white;

        [Inject]
        public void Constructor(SignalBus signalBus, UITemplateInventoryDataController uiTemplateInventoryDataController)
        {
            this.signalBus                         = signalBus;
            this.uiTemplateInventoryDataController = uiTemplateInventoryDataController;
            
            this.Initialize();
            this.ResetState();
        }

        private void Initialize()
        {
            this.signalBus.Subscribe<UpdateCurrencySignal>(this.UpdateCurrency);
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
            if (this.signalBus == null)
            {
                throw new Exception($"Please inject for GameObject: {this.gameObject.name}");
            }
            this.signalBus.Unsubscribe<UpdateCurrencySignal>(this.UpdateCurrency);
        }

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
        public override string CurrencyKey => this.currencyId;
    }
}