namespace TheOneStudio.UITemplate.UITemplate.ThirdPartyServices.AnalyticEvents.Wido
{
    using Core.AnalyticServices.Data;

    public class ShowRewardedAds : IEvent
    {
        public bool   internet_available;
        public string placement;
        
        public ShowRewardedAds(bool internetAvailable, string placement)
        {
            this.internet_available = internetAvailable;
            this.placement          = placement;
        }
    }
}