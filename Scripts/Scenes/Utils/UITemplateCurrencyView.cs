namespace TheOneStudio.UITemplate.UITemplate.Scenes.Utils
{
    using TheOneStudio.UITemplate.UITemplate.Signals;
    using TMPro;
    using UnityEngine;
    using Zenject;

    public class UITemplateCurrencyView : MonoBehaviour
    {
        [SerializeField] private TMP_Text currencyValueText;
        [SerializeField] private string   currencyId;

        public void Subscribe(SignalBus signalBus, int initValue)
        {
            signalBus.Subscribe<UpdateCurrencySignal>(this.UpdateCurrency);
            this.UpdateData(initValue);
        }

        private void UpdateData(int initValue) { this.currencyValueText.text = initValue.ToString(); }

        public void Unsubscribe(SignalBus signalBus) { signalBus.Unsubscribe<UpdateCurrencySignal>(this.UpdateCurrency); }

        private void UpdateCurrency(UpdateCurrencySignal obj)
        {
            if (!this.currencyId.Equals(obj.Id)) return;
            this.UpdateData(obj.FinalValue);
        }
    }
}