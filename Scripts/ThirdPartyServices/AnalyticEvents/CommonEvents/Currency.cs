namespace TheOneStudio.UITemplate.UITemplate.ThirdPartyServices.AnalyticEvents.CommonEvents
{
    using Core.AnalyticServices.Data;

    public class EarnVirtualCurrency : IEvent
    {
        public string virtualCurrencyName;
        public long   value;
        public string source;
        
        public EarnVirtualCurrency(string virtualCurrencyName, long value, string source)
        {
            this.virtualCurrencyName = virtualCurrencyName;
            this.value               = value;
            this.source              = source;
        }
    }
    
    public class SpendVirtualCurrency : IEvent
    {
        public string virtualCurrencyName;
        public long   value;
        public string itemName;
        
        public SpendVirtualCurrency(string virtualCurrencyName, long value, string itemName)
        {
            this.virtualCurrencyName = virtualCurrencyName;
            this.value               = value;
            this.itemName              = itemName;
        }
    }
}