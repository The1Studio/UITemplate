namespace TheOneStudio.UITemplate.UITemplate.Scripts.ThirdPartyServices.AnalyticEvents.ABI
{
    using Core.AnalyticServices.Data;

    public class AdInterDownloaded : IEvent
    {
        public string place;
        public long   loadingMilis;
        public AdInterDownloaded(string place, long loadingMilis)
        {
            this.place        = place;
            this.loadingMilis = loadingMilis;
        }
    }
}