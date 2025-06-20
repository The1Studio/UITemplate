namespace TheOneStudio.UITemplate.UITemplate.ThirdPartyServices.AnalyticEvents.Sonat
{
    using Core.AnalyticServices.Data;
    using UnityEngine.Scripting;

    internal sealed class SpendVirtualCurrency : IEvent
    {
        public string VirtualCurrencyName { get; }
        public float  Value               { get; }
        public string Location            { get; }
        public string Screen              { get; }
        public string ItemType            { get; }
        public string ItemId              { get; }

        [Preserve]
        public SpendVirtualCurrency(string virtualCurrencyName, float value, string location, string screen, string itemType, string itemId)
        {
            this.VirtualCurrencyName = virtualCurrencyName;
            this.Value               = value;
            this.Location            = location;
            this.Screen              = screen;
            this.ItemType            = itemType;
            this.ItemId              = itemId;
        }
    }
}