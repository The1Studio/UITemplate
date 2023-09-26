namespace TheOneStudio.UITemplate.UITemplate.Scripts.ThirdPartyServices
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Core.AdsServices;
    using Core.AdsServices.Signals;
    using Cysharp.Threading.Tasks;
    using GameFoundation.Scripts.Utilities.LogService;
    using TheOneStudio.UITemplate.UITemplate.Models.Controllers;
    using TheOneStudio.UITemplate.UITemplate.Services.RewardHandle;
    using TheOneStudio.UITemplate.UITemplate.Services.Toast;
    using Zenject;

    public class UITemplateAdServiceWrapper : IInitializable
    {
        #region inject

        private readonly IAdServices             adServices;
        private readonly List<IMRECAdService>    mrecAdServices;
        private readonly UITemplateAdsController uiTemplateAdsController;
        private readonly IAOAAdService           aoaAdService;
        private readonly IBackFillAdsService     backFillAdsService;
        private readonly ToastController         toastController;
        private readonly ILogService             logService;
        private readonly AdServicesConfig        adServicesConfig;
        private readonly SignalBus               signalBus;

        #endregion

        private DateTime LastEndInterstitial;
        private bool     isBannerLoaded     = false;
        private bool     isShowBannerAd     = true;

        public bool isWatchingVideoAds = false;

        public UITemplateAdServiceWrapper(ILogService logService, AdServicesConfig adServicesConfig, SignalBus signalBus, IAdServices adServices, List<IMRECAdService> mrecAdServices,
            UITemplateAdsController uiTemplateAdsController,
            IAOAAdService aoaAdService, IBackFillAdsService backFillAdsService, ToastController toastController)
        {
            this.adServices              = adServices;
            this.mrecAdServices          = mrecAdServices;
            this.uiTemplateAdsController = uiTemplateAdsController;
            this.aoaAdService            = aoaAdService;
            this.backFillAdsService      = backFillAdsService;
            this.toastController         = toastController;
            this.logService              = logService;
            this.adServicesConfig        = adServicesConfig;
            this.signalBus               = signalBus;
        }

        #region banner

        public virtual async void ShowBannerAd()
        {
            if (this.adServices.IsRemoveAds())
            {
                return;
            }

            this.isShowBannerAd = true;
            await UniTask.WaitUntil(() => this.adServices.IsAdsInitialized());
            if (this.isShowBannerAd)
            {
                this.adServices.ShowBannerAd();
            }
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
            this.signalBus.Subscribe<BannerAdPresentedSignal>(this.OnBannerAdPresented);
            this.signalBus.Subscribe<UITemplateAddRewardsSignal>(this.OnRemoveAdsComplete);
            this.signalBus.Subscribe<RewardedAdCompletedSignal>(this.OnRewardedVideoComplete);
            this.signalBus.Subscribe<RewardedSkippedSignal>(this.OnRewardedVideoSkipped);
        }
        
        private void OnRewardedVideoSkipped(RewardedSkippedSignal obj)
        {
            this.isWatchingVideoAds = false;
        }
        
        private void OnRewardedVideoComplete(RewardedAdCompletedSignal obj)
        {
            this.isWatchingVideoAds = false;
        }

        private void OnBannerAdPresented(BannerAdPresentedSignal obj)
        {
            if (this.adServices.IsRemoveAds())
            {
                this.adServices.DestroyBannerAd();
            } else if (!this.isShowBannerAd)
            {
                this.adServices.HideBannedAd();
            }
        }

        private void OnInterstitialAdClosedHandler() { this.LastEndInterstitial = DateTime.Now; }

        #region InterstitialAd

        public virtual bool IsInterstitialAdReady(string place) { return this.adServices.IsInterstitialAdReady(place); }

        public virtual bool ShowInterstitialAd(string place, bool force = false)
        {
            if (this.adServices.IsRemoveAds()) return false;

            if ((DateTime.Now - this.LastEndInterstitial).TotalSeconds < this.adServicesConfig.InterstitialAdInterval && !force)
            {
                this.logService.Warning("InterstitialAd was not passed interval");

                return false;
            }

            this.signalBus.Fire(new InterstitialAdEligibleSignal(place));

            if (!this.adServices.IsInterstitialAdReady(place))
            {
                if (!this.backFillAdsService.IsInterstitialAdReady(place))
                {
                    this.logService.Warning("InterstitialAd was not loaded");

                    return false;
                }

                this.signalBus.Fire(new InterstitialAdCalledSignal(place));
                this.uiTemplateAdsController.UpdateWatchedInterstitialAds();
                this.aoaAdService.IsResumedFromAdsOrIAP = true;
                this.backFillAdsService.ShowInterstitialAd(place);
                return true;
            }

            this.signalBus.Fire(new InterstitialAdCalledSignal(place));
            this.uiTemplateAdsController.UpdateWatchedInterstitialAds();
            this.aoaAdService.IsResumedFromAdsOrIAP = true;
            this.adServices.ShowInterstitialAd(place);

            return true;
        }

        public virtual void LoadInterstitialAd(string place) { this.signalBus.Fire(new InterstitialAdDownloadedSignal(place)); }

        #endregion

        #region RewardAd

        public virtual void ShowRewardedAd(string place, Action onComplete, Action onFail = null)
        {
            this.signalBus.Fire(new RewardedAdEligibleSignal(place));

            if (!this.adServices.IsRewardedAdReady(place))
            {
                this.logService.Warning("Rewarded was not loaded");
                onFail?.Invoke();
                this.toastController.SetContent("There is no Ads!");

                return;
            }

            this.signalBus.Fire(new RewardedAdCalledSignal(place));
            this.uiTemplateAdsController.UpdateWatchedRewardedAds();
            this.aoaAdService.IsResumedFromAdsOrIAP = true;
            this.isWatchingVideoAds                 = true;
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

        private void OnRemoveAdsComplete(UITemplateAddRewardsSignal signal)
        {
            if (!signal.RewardItemDatas.Keys.Contains("remove_ads")) return;
            foreach (var mrecAdService in this.mrecAdServices)
            {
                mrecAdService.HideAllMREC();
            }

            this.adServices.DestroyBannerAd();
        }

        public bool IsRemovedAds => this.adServices.IsRemoveAds();
    }
}