namespace TheOneStudio.UITemplate.UITemplate.ThirdPartyServices.AnalyticEvents.Rocket
{
    using Core.AnalyticServices.Data;

    public class TotalVirtualCurrencyEarned : IEvent
    {
        public string CurrencyName;
        public long Amount;
        
        public TotalVirtualCurrencyEarned(string currencyName, long amount)
        {
            this.CurrencyName = currencyName;
            this.Amount = amount;
        }
    }
}