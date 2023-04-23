namespace TheOneStudio.UITemplate.UITemplate.Models.Controllers
{
    public class UITemplateAdsController : IUITemplateControllerData
    {
        private readonly UITemplateAdsData uiTemplateAdsData;

        public UITemplateAdsController(UITemplateAdsData uiTemplateAdsData) { this.uiTemplateAdsData = uiTemplateAdsData; }

        public void UpdateWatchedInterstitialAds() { this.uiTemplateAdsData.WatchedInterstitialAds++; }
        public void UpdateWatchedRewardedAds()     { this.uiTemplateAdsData.WatchedRewardedAds++; }
    }
}