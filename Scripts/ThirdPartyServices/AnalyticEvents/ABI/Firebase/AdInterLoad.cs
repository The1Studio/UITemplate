namespace TheOneStudio.UITemplate.UITemplate.ThirdPartyServices.AnalyticEvents.ABI.Firebase
{
    using Core.AnalyticServices.Data;

    public class AdInterLoad : IEvent
    {
        public string Place;

        public AdInterLoad(string place) { this.Place = place; }
    }
}