namespace TheOneStudio.UITemplate.UITemplate.Scripts.ThirdPartyServices
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Core.AdsServices;
    using Core.AdsServices.Signals;
    using Cysharp.Threading.Tasks;
    using GameFoundation.Scripts.Utilities.LogService;
    using TheOneStudio.UITemplate.UITemplate.Models;
    using TheOneStudio.UITemplate.UITemplate.Scripts.Signals;
    using TheOneStudio.UITemplate.UITemplate.Signals;
    using Zenject;

    public class UITemplateAdServiceConfig
    {
        public long InterstitialAdInterval { get; set; }
    }

    public class UITemplateAdServiceWrapper : IInitializable
    {
        #region inject

        private readonly IAdServices               adServices;
        private readonly List<IMRECAdService>      mrecAdServices;
        private readonly UITemplateAdsData         uiTemplateAdsData;
        private readonly UITemplateAdServiceConfig config;
        private readonly IAOAAdService             aoaAdService;
        private readonly ILogService               logService;
        private readonly SignalBus                 signalBus;

        #endregion

        private DateTime LastEndInterstitial;
        private bool     isBannerLoaded = false;
        private bool     isShowBannerAd;

        public UITemplateAdServiceWrapper(ILogService               logService, SignalBus signalBus, IAdServices adServices, List<IMRECAdService> mrecAdServices, UITemplateAdsData uiTemplateAdsData,
                                          UITemplateAdServiceConfig config, IAOAAdService aoaAdService)
        {
            this.adServices        = adServices;
            this.mrecAdServices    = mrecAdServices;
            this.uiTemplateAdsData = uiTemplateAdsData;
            this.config            = config;
            this.aoaAdService      = aoaAdService;
            this.logService        = logService;
            this.signalBus         = signalBus;
        }

        #region banner

        public virtual async void ShowBannerAd()
        {
            this.isShowBannerAd = true;
            await UniTask.WaitUntil(() => this.adServices.IsAdsInitialized());
            this.ShowBannerInterval();
        }

        private async void ShowBannerInterval()
        {
            if (this.isShowBannerAd)
            {
                this.adServices.ShowBannerAd();   
            }
            await UniTask.Delay(TimeSpan.FromSeconds(5));
            this.ShowBannerInterval();
        }
        
        public virtual void HideBannerAd()
        {
            this.isShowBannerAd = false;
            this.adServices.HideBannedAd();
        }

        #endregion
        
        public void Initialize()
        {
            this.signalBus.Subscribe<InterstitialAdClosedSignal>(this.OnInterstitialAdClosedHandler);
        }
        private void OnInterstitialAdClosedHandler()
        {
            this.LastEndInterstitial = DateTime.Now;
        }

        #region InterstitialAd

        public virtual bool IsInterstitialAdReady(string place) { return this.adServices.IsInterstitialAdReady(place); }

        public virtual bool ShowInterstitialAd(string place, bool force = false)
        {
            if ((DateTime.Now - this.LastEndInterstitial).TotalSeconds < this.config.InterstitialAdInterval && !force)
            {
                this.logService.Warning("InterstitialAd was not passed interval");

                return false;
            }

            this.signalBus.Fire(new InterstitialAdEligibleSignal(place));
            if (!this.adServices.IsInterstitialAdReady(place))
            {
                this.logService.Warning("InterstitialAd was not loaded");

                return false;
            }

            this.signalBus.Fire(new InterstitialAdCalledSignal(place));
            this.uiTemplateAdsData.WatchedInterstitialAds++;
            this.aoaAdService.IsResumedFromAdsOrIAP = true;
            this.adServices.ShowInterstitialAd(place);

            return true;
        }

        public virtual void LoadInterstitialAd(string place) { this.signalBus.Fire(new InterstitialAdDownloadedSignal(place)); }

        #endregion

        #region RewardAd

        public virtual void ShowRewardedAd(string place, Action onComplete)
        {
            this.signalBus.Fire(new RewardedAdEligibleSignal(place));
            if (!this.adServices.IsRewardedAdReady(place))
            {
                this.logService.Warning("Rewarded was not loaded");

                return;
            }

            this.signalBus.Fire(new RewardedAdCalledSignal(place));
            this.uiTemplateAdsData.WatchedRewardedAds++;
            this.aoaAdService.IsResumedFromAdsOrIAP = true;
            this.adServices.ShowRewardedAd(place, onComplete);
        }

        public virtual bool IsRewardedAdReady(string place) { return this.adServices.IsRewardedAdReady(place); }

        public virtual void RewardedAdOffer(string place) { this.signalBus.Fire(new RewardedAdOfferSignal(place)); }

        #endregion

        public virtual void ShowMREC(AdViewPosition adViewPosition)
        {
            if (this.adServices.IsRemoveAds()) return;

            var mrecAdService = this.mrecAdServices.FirstOrDefault(service => service.IsMRECReady(adViewPosition));
            
            if (mrecAdService != null)
            {
                mrecAdService.ShowMREC(adViewPosition);

                if (adViewPosition == AdViewPosition.BottomCenter)
                {
                    this.adServices.HideBannedAd();
                }
            }
        }

        public virtual void HideMREC(AdViewPosition adViewPosition)
        {
            var mrecAdServices = this.mrecAdServices.Where(service => service.IsMRECReady(adViewPosition)).ToList();
            if (mrecAdServices.Count > 0)
            {
                foreach (var mrecAdService in mrecAdServices)
                {
                    mrecAdService.HideMREC(adViewPosition);
                }

                if (adViewPosition == AdViewPosition.BottomCenter)
                {
                    this.adServices.ShowBannerAd();
                }
            }
        }
    }
}