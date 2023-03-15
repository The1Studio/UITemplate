namespace TheOneStudio.UITemplate.UITemplate.ThirdPartyServices.AnalyticEvents.ABI.Firebase
{
    using Core.AnalyticServices.Data;

    public class AdsRewardComplete : IEvent
    {
        public string Place;

        public AdsRewardComplete(string place) { this.Place = place; }
    }
}