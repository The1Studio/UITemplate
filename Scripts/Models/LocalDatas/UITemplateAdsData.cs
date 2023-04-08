namespace TheOneStudio.UITemplate.UITemplate.Models
{
    using GameFoundation.Scripts.Interfaces;
    using Sirenix.Serialization;

    public class UITemplateAdsData : ILocalData
    {
        [OdinSerialize]
        public int WatchedInterstitialAds { get; set; }

        [OdinSerialize]
        public int WatchedRewardedAds { get; set; }

        public void Init()
        {
        }
    }
}