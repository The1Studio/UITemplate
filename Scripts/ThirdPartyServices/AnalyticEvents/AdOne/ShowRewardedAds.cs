namespace TheOneStudio.UITemplate.UITemplate.Scripts.ThirdPartyServices.AnalyticEvents.AdOne
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