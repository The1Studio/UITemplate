namespace TheOneStudio.UITemplate.UITemplate.Models
{
    using GameFoundation.Scripts.Interfaces;

    public class UITemplateAdsData : ILocalData
    {
        internal int WatchedInterstitialAds { get; set; }
        internal int WatchedRewardedAds     { get; set; }

        public void Init() { }
    }
}