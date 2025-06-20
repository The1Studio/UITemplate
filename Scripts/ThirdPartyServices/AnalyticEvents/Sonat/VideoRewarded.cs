namespace TheOneStudio.UITemplate.UITemplate.ThirdPartyServices.AnalyticEvents.Sonat
{
    using Core.AnalyticServices.Data;
    using UnityEngine.Scripting;
    [Preserve]
    internal sealed class VideoRewarded : IEvent
    {
        public string Mode      { get; }
        public string Level     { get; }
        public string Phase     { get; }
        public string Location  { get; }
        public string Placement { get; }
        public string ItemType  { get; }
        public string ItemId    { get; }

        public VideoRewarded(string mode, string level, string phase, string location, string placement, string itemType, string itemId)
        {
            this.Mode      = mode;
            this.Level     = level;
            this.Phase     = phase;
            this.Location  = location;
            this.Placement = placement;
            this.ItemType  = itemType;
            this.ItemId    = itemId;
        }
    }
}