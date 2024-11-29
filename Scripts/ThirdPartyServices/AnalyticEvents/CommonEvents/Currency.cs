namespace TheOneStudio.UITemplate.UITemplate.ThirdPartyServices.AnalyticEvents.CommonEvents
{
    using Core.AnalyticServices.Data;

    public class EarnVirtualCurrency : IEvent
    {
        public string virtualCurrencyName;
        public long   value;
        public string placement;
        public int    level;

        public EarnVirtualCurrency(string virtualCurrencyName, long value, string placement, int level)
        {
            this.virtualCurrencyName = virtualCurrencyName;
            this.value               = value;
            this.placement           = placement;
            this.level               = level;
        }
    }

    public class SpendVirtualCurrency : IEvent
    {
        public string virtualCurrencyName;
        public long   value;
        public string placement;
        public int    level;

        public SpendVirtualCurrency(string virtualCurrencyName, long value, string placement, int level)
        {
            this.virtualCurrencyName = virtualCurrencyName;
            this.value               = value;
            this.placement           = placement;
            this.level              = level;
        }
    }
}