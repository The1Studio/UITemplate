namespace TheOneStudio.UITemplate.UITemplate.Models
{
    using GameFoundation.Scripts.Interfaces;

    public class UITemplateAdsData : ILocalData
    {
        public int WatchedInterstitialAds { get; set; }
        public int WatchedRewardedAds     { get; set; }

        public void Init() { }
    }
}