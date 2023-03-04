namespace TheOneStudio.UITemplate.UITemplate.Scripts.ThirdPartyServices
{
    using System;
    using Core.AdsServices;
    using Cysharp.Threading.Tasks;
    using GameFoundation.Scripts.Utilities.LogService;
    using TheOneStudio.UITemplate.UITemplate.Models;
    using TheOneStudio.UITemplate.UITemplate.Signals;
    using Zenject;

    public class UITemplateAdServiceWrapper
    {
        #region inject

        private readonly IAdServices       adServices;
        private readonly UITemplateAdsData uiTemplateAdsData;
        private readonly ILogService       logService;
        private readonly SignalBus         signalBus;

        #endregion

        public UITemplateAdServiceWrapper(ILogService logService, SignalBus signalBus, IAdServices adServices, UITemplateAdsData uiTemplateAdsData)
        {
            this.adServices        = adServices;
            this.uiTemplateAdsData = uiTemplateAdsData;
            this.logService        = logService;
            this.signalBus         = signalBus;
        }

        public async void ShowBannerAd()
        {
            await UniTask.WaitUntil(() => this.adServices.IsAdsInitialized());
            this.adServices.ShowBannerAd();
        }

        public void ShowInterstitialAd(string place)
        {
            if (!this.adServices.IsInterstitialAdReady(place))
            {
                this.logService.Warning("InterstitialAd was not loaded");

                return;
            }

            this.signalBus.Fire(new InterstitialAdShowedSignal(place));
            this.uiTemplateAdsData.WatchedInterstitialAds++;;
            this.adServices.ShowInterstitialAd(place);
        }

        public void ShowRewardedAd(string place, Action onComplete)
        {
            if (!this.adServices.IsRewardedAdReady(place))
            {
                this.logService.Warning("Rewarded was not loaded");

                return;
            }

            this.signalBus.Fire(new RewardedAdShowedSignal(place));
            this.uiTemplateAdsData.WatchedRewardedAds++;;
            this.adServices.ShowRewardedAd(place, onComplete);
        }

        public bool IsRewardedAdReady(string place) { return this.adServices.IsRewardedAdReady(place); }

        public bool IsInterstitialAdReady(string place) { return this.adServices.IsInterstitialAdReady(place); }
    }
}