namespace TheOneStudio.UITemplate.UITemplate.ThirdPartyServices.AnalyticEvents.Sonat
{
    using Core.AnalyticServices.Data;

    internal sealed class InterstitialShow : IEvent
    {
        public string Location  { get; }
        public string Screen    { get; }
        public string Placement { get; }
        public string Level     { get; }
        public string Mode      { get; }

        public InterstitialShow(string location, string screen, string placement, string level, string mode)
        {
            this.Location  = location;
            this.Screen    = screen;
            this.Placement = placement;
            this.Level     = level;
            this.Mode      = mode;
        }
    }
}