namespace TheOneStudio.UITemplate.UITemplate.ThirdPartyServices.AnalyticEvents.Rocket
{
    using Core.AnalyticServices.Data;

    public class DaysPlayed : IEvent
    {
        public int Days;

        public DaysPlayed(int days)
        {
            this.Days = days;
        }
    }
}