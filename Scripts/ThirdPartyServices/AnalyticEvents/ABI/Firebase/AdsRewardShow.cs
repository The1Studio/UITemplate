namespace TheOneStudio.UITemplate.UITemplate.ThirdPartyServices.AnalyticEvents.ABI.Firebase
{
    using Core.AnalyticServices.Data;

    public class AdsRewardShow: IEvent
    {
        public int    Level;
        public string Place;

        public AdsRewardShow(int level, string place)
        {
            this.Level = level;
            this.Place = place;
        }
    }
}