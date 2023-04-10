namespace TheOneStudio.UITemplate.UITemplate.Scripts.Signals
{
    public class TotalVirtualCurrencyEarnedSignal
    {
        public string CurrencyName;
        public long Amount;
        
        public TotalVirtualCurrencyEarnedSignal(string currencyName, int amount)
        {
            this.CurrencyName = currencyName;
            this.Amount = amount;
        }
    }
}