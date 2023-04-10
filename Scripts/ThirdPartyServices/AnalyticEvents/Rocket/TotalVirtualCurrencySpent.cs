namespace TheOneStudio.UITemplate.UITemplate.ThirdPartyServices.AnalyticEvents.Rocket
{
    using Core.AnalyticServices.Data;

    public class TotalVirtualCurrencySpent : IEvent
    {
        public long Amount;
        public string CurrencyName;
        
        public TotalVirtualCurrencySpent(string currencyName, long amount)
        {
            this.CurrencyName = currencyName;
            this.Amount = amount;
        }
            
    }
}