namespace TheOneStudio.UITemplate.UITemplate.ThirdPartyServices.AnalyticEvents.ABI.Firebase
{
    using Core.AnalyticServices.Data;

    public class AdsRewardFail: IEvent
    {
        public string Place;

        public AdsRewardFail(string place) { this.Place = place; }
    }
}