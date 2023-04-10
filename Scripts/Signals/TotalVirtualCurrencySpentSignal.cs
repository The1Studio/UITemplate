namespace TheOneStudio.UITemplate.UITemplate.Scripts.Signals
{
    public class TotalVirtualCurrencySpentSignal
    {
        public string CurrencyName;
        public long Amount;
        
        public TotalVirtualCurrencySpentSignal(string currencyName, int amount)
        {
            this.CurrencyName = currencyName;
            this.Amount = amount;
        }
    }
}