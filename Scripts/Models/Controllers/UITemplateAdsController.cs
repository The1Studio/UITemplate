namespace TheOneStudio.UITemplate.UITemplate.Models.Controllers
{
    public class UITemplateAdsController : IUITemplateControllerData
    {
        private readonly UITemplateAdsData uiTemplateAdsData;

        public int WatchInterstitialAds => this.uiTemplateAdsData.WatchedInterstitialAds;
        public int WatchRewardedAds     => this.uiTemplateAdsData.WatchedRewardedAds;
        public UITemplateAdsController(UITemplateAdsData uiTemplateAdsData) { this.uiTemplateAdsData = uiTemplateAdsData; }

        public void UpdateWatchedInterstitialAds() { this.uiTemplateAdsData.WatchedInterstitialAds++; }
        public void UpdateWatchedRewardedAds()     { this.uiTemplateAdsData.WatchedRewardedAds++; }
    }
}