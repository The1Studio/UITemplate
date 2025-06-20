namespace TheOneStudio.UITemplate.UITemplate.ThirdPartyServices.AnalyticEvents.Sonat
{
    using Core.AnalyticServices.Data;
    using UnityEngine.Scripting;

    internal sealed class ShowInterstitial : IEvent
    {
        public string Location  { get; }
        public string Screen    { get; }
        public string Placement { get; }
        public string Level     { get; }
        public string Mode      { get; }

        [Preserve]
        public ShowInterstitial(string location, string screen, string placement, string level, string mode)
        {
            this.Location  = location;
            this.Screen    = screen;
            this.Placement = placement;
            this.Level     = level;
            this.Mode      = mode;
        }
    }
}