namespace TheOneStudio.UITemplate.UITemplate.ThirdPartyServices.AnalyticEvents.ABI.Firebase
{
    using Core.AnalyticServices.Data;

    public class AdInterShow: IEvent
    {
        public int    Level;
        public string Place;

        public AdInterShow(int level, string place)
        {
            this.Level = level;
            this.Place = place;
        }
    }
}