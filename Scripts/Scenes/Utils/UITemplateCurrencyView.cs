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

        public void Subscribe(SignalBus signalBus) { signalBus.Subscribe<UpdateCurrencySignal>(this.UpdateCurrency); }

        public void Unsubscribe(SignalBus signalBus) { signalBus.Unsubscribe<UpdateCurrencySignal>(this.UpdateCurrency); }

        private void UpdateCurrency(UpdateCurrencySignal obj)
        {
            if (!this.currencyId.Equals(obj.Id)) return;
            this.currencyValueText.text = obj.FinalValue.ToString();
        }
    }
}