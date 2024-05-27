namespace TheOneStudio.UITemplate.UITemplate.Scripts.ThirdPartyServices.AnalyticEvents.ABI
{
    using Core.AnalyticServices.Data;

    public class AdsRewardedDownloaded : IEvent
    {
        public string place;
        public long   loadingMilis;
        
        public AdsRewardedDownloaded(string place, long loadingMilis)
        {
            this.place        = place;
            this.loadingMilis = loadingMilis;
        }
    }
}