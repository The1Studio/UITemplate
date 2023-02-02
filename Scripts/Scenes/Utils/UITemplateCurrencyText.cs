namespace UITemplate.Scripts.Scenes.Popups
{
    using TMPro;
    using UITemplate.Scripts.Signals;
    using Zenject;

    public class UITemplateCurrencyText : TMP_Text
    {
        public string CurrencyId;
        
        public void Subscribe(SignalBus signalBus)
        {
            signalBus.Subscribe<UpdateCurrencySignal>(this.UpdateCurrency);
        }
        
        public void Unsubscribe(SignalBus signalBus)
        {
            signalBus.Unsubscribe<UpdateCurrencySignal>(this.UpdateCurrency);
        }
        
        private void UpdateCurrency(UpdateCurrencySignal obj)
        {
            if (!this.CurrencyId.Equals(obj.Id)) return;
            this.text = obj.FinalValue.ToString();
        }
    }
}