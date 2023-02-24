namespace TheOneStudio.UITemplate.UITemplate.ThirdPartyServices
{
    using System;
    using Core.AdsServices;
    using GameFoundation.Scripts.Utilities.LogService;
    using TheOneStudio.UITemplate.UITemplate.Signals;
    using Zenject;

    public class AdServiceWrapper
    {
        #region inject

        private readonly IAdServices   adServices;
        private readonly IAOAAdService aoaAdService;
        private readonly ILogService   logService;
        private readonly SignalBus     signalBus;

        #endregion

        public AdServiceWrapper(IAOAAdService aoaAdService, ILogService logService, SignalBus signalBus, IAdServices adServices)
        {
            this.adServices   = adServices;
            this.aoaAdService = aoaAdService;
            this.logService   = logService;
            this.signalBus    = signalBus;
        }

        public void ShowBannerAd()
        {
            this.adServices.ShowBannerAd();
        }

        public void ShowInterstitialAd(string place)
        {
            if (!this.adServices.IsInterstitialAdReady())
            {
                this.logService.Warning("InterstitialAd was not loaded");

                return;
            }

            this.signalBus.Fire(new InterstitialAdShowedSignal(place));
            this.adServices.ShowInterstitialAd(place);
        }

        public void ShowRewardedAd(string place, Action onComplete)
        {
            if (!this.adServices.IsRewardedAdReady())
            {
                this.logService.Warning("Rewarded was not loaded");

                return;
            }

            this.signalBus.Fire(new RewardedAdShowedSignal(place));
            this.aoaAdService.OpenInterstitialAdHandler();
            this.adServices.ShowRewardedAd(place, onComplete);
        }
    }
}