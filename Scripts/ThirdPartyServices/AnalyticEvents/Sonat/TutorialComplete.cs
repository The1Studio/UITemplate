namespace TheOneStudio.UITemplate.UITemplate.ThirdPartyServices.AnalyticEvents.Sonat
{
    using Core.AnalyticServices.Data;

    internal sealed class TutorialComplete : IEvent
    {
        public string Placement { get; }
        public int    Step      { get; }

        public TutorialComplete(string placement, int step)
        {
            this.Placement = placement;
            this.Step      = step;
        }
    }
}