namespace TheOneStudio.UITemplate.UITemplate.Models.Controllers
{
    using System;
    using System.Linq;
    using UnityEngine.Scripting;

    public class UITemplateAdsController : IUITemplateControllerData
    {
        private readonly UITemplateAdsData uiTemplateAdsData;

        [Preserve]
        public UITemplateAdsController(UITemplateAdsData uiTemplateAdsData)
        {
            this.uiTemplateAdsData = uiTemplateAdsData;
        }

        public int WatchInterstitialAds => this.uiTemplateAdsData.WatchedInterstitialAds;
        public int WatchRewardedAds     => this.uiTemplateAdsData.WatchedRewardedAds;
        public int WatchedAdsCount      => this.uiTemplateAdsData.WatchedInterstitialAds + this.uiTemplateAdsData.WatchedRewardedAds;
        public DateTime AdFreeTrialExpirationTime => this.uiTemplateAdsData.AdFreeTrialExpirationTime;

        public void UpdateWatchedInterstitialAds()
        {
            this.uiTemplateAdsData.WatchedInterstitialAds++;
        }

        public void UpdateWatchedRewardedAds()
        {
            this.uiTemplateAdsData.WatchedRewardedAds++;
        }

        public void CountAdsImpression(double revenue)
        {
            this.uiTemplateAdsData.InterstitialAndRewardedRevenue.Add(revenue);
        }

        public bool TryGetCircleSumInterstitialAndRewardedAdsRevenue(int circle, out double sum)
        {
            sum = 0;
            var adsCount = this.uiTemplateAdsData.InterstitialAndRewardedRevenue.Count;

            if (adsCount > 0 && adsCount % circle != 0) return false;
            sum = this.uiTemplateAdsData.InterstitialAndRewardedRevenue.TakeLast(circle).Sum();

            return true;
        }

        public void AddAdFreeTrialTime(TimeSpan duration)
        {
            if (this.uiTemplateAdsData.AdFreeTrialExpirationTime < DateTime.Now)
            {
                this.uiTemplateAdsData.AdFreeTrialExpirationTime = DateTime.Now.Add(duration);
            }
            else
            {
                this.uiTemplateAdsData.AdFreeTrialExpirationTime = this.uiTemplateAdsData.AdFreeTrialExpirationTime.Add(duration);
            }
        }

        public TimeSpan GetRemainingAdFreeTrialTime()
        {
            if (!this.IsAdFreeTrialActive()) return TimeSpan.Zero;

            var remaining = this.AdFreeTrialExpirationTime - DateTime.Now;
            return remaining > TimeSpan.Zero ? remaining : TimeSpan.Zero;
        }

        public void ResetAdFreeTrial()
        {
            this.uiTemplateAdsData.AdFreeTrialExpirationTime = DateTime.MinValue;
        }

        public bool IsAdFreeTrialActive()
        {
            return DateTime.Now <= this.uiTemplateAdsData.AdFreeTrialExpirationTime;
        }
    }
}