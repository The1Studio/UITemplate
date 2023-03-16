namespace TheOneStudio.UITemplate.UITemplate.ThirdPartyServices.AnalyticEvents.ABI
{
    using Core.AnalyticServices.Data;

    public class EarnVirtualCurrency : IEvent
    {
        public string VirtualCurrencyName;
        public long   Value;
        public string Source;

        public EarnVirtualCurrency(string virtualCurrencyName, long value, string source)
        {
            this.VirtualCurrencyName = virtualCurrencyName;
            this.Value               = value;
            this.Source              = source;
        }
    }
}