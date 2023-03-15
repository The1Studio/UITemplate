namespace TheOneStudio.UITemplate.UITemplate.ThirdPartyServices.AnalyticEvents.ABI.Firebase
{
    using Core.AnalyticServices.Data;

    public class AdsRewardClick: IEvent
    {
        public string Place;

        public AdsRewardClick(string place) { this.Place = place; }
    }
}