namespace TheOneStudio.UITemplate.UITemplate.Scripts.ThirdPartyServices
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Core.AdsServices;
    using Core.AdsServices.CollapsibleBanner;
    using Core.AdsServices.Signals;
    using Cysharp.Threading.Tasks;
    using GameFoundation.Scripts.UIModule.ScreenFlow.BaseScreen.Presenter;
    using GameFoundation.Scripts.UIModule.ScreenFlow.Managers;
    using GameFoundation.Scripts.UIModule.ScreenFlow.Signals;
    using GameFoundation.Scripts.Utilities.ApplicationServices;
    using GameFoundation.Scripts.Utilities.LogService;
    using ServiceImplementation.Configs;
    using ServiceImplementation.Configs.Ads;
    using ServiceImplementation.IAPServices.Signals;
    using TheOneStudio.UITemplate.UITemplate.Models.Controllers;
    using TheOneStudio.UITemplate.UITemplate.Services.BreakAds;
    using TheOneStudio.UITemplate.UITemplate.Services.Toast;
    using TheOneStudio.UITemplate.UITemplate.Signals;
    using UnityEngine;
    using Zenject;

    public class UITemplateAdServiceWrapper : IInitializable, ITickable
    {
        #region inject

        private readonly IAdServices                         adServices;
        private readonly List<IMRECAdService>                mrecAdServices;
        private readonly UITemplateAdsController             uiTemplateAdsController;
        private readonly UITemplateGameSessionDataController gameSessionDataController;
        private readonly List<IAOAAdService>                 aoaAdServices;
        private readonly IBackFillAdsService                 backFillAdsService;
        private readonly ToastController                     toastController;
        private readonly UITemplateLevelDataController       levelDataController;
        private readonly ThirdPartiesConfig                  thirdPartiesConfig;
        private readonly IScreenManager                      screenManager;
        private readonly ICollapsibleBannerAd                collapsibleBannerAd;
        private readonly ILogService                         logService;
        private readonly AdServicesConfig                    adServicesConfig;
        private readonly SignalBus                           signalBus;

        #endregion

        //Interstitial
        private float        totalNoAdsPlayingTime;
        private Action<bool> onInterstitialFinishedAction;
        private int          totalInterstitialAdsShowedInSession;

        //Banner
        private bool IsShowBannerAd         { get; set; }
        private bool IsCheckFirstScreenShow { get; set; }

        //AOA
        private DateTime StartLoadingAOATime;
        private DateTime StartBackgroundTime;
        private bool     IsResumedFromAdsOrIAP;
        private bool     IsCheckedShowFirstOpen { get; set; } = false;
        public  bool     IsOpenedAOAFirstOpen   { get; private set; }

        public UITemplateAdServiceWrapper(ILogService logService, AdServicesConfig adServicesConfig, SignalBus signalBus, IAdServices adServices, List<IMRECAdService> mrecAdServices,
            UITemplateAdsController uiTemplateAdsController, UITemplateGameSessionDataController gameSessionDataController,
            List<IAOAAdService> aoaAdServices, IBackFillAdsService backFillAdsService, ToastController toastController, UITemplateLevelDataController levelDataController,
            ThirdPartiesConfig thirdPartiesConfig, IScreenManager screenManager, ICollapsibleBannerAd collapsibleBannerAd)
        {
            this.adServices                = adServices;
            this.mrecAdServices            = mrecAdServices;
            this.uiTemplateAdsController   = uiTemplateAdsController;
            this.gameSessionDataController = gameSessionDataController;
            this.aoaAdServices             = aoaAdServices;
            this.backFillAdsService        = backFillAdsService;
            this.toastController           = toastController;
            this.levelDataController       = levelDataController;
            this.thirdPartiesConfig        = thirdPartiesConfig;
            this.screenManager             = screenManager;
            this.collapsibleBannerAd       = collapsibleBannerAd;
            this.logService                = logService;
            this.adServicesConfig          = adServicesConfig;
            this.signalBus                 = signalBus;
        }

        public void Initialize()
        {
            this.signalBus.Subscribe<InterstitialAdClosedSignal>(this.OnInterstitialAdClosedHandler);
            this.signalBus.Subscribe<InterstitialAdDisplayedFailedSignal>(this.OnInterstitialDisplayedFailedHandler);
            this.signalBus.Subscribe<BannerAdPresentedSignal>(this.OnBannerAdPresented);
            this.signalBus.Subscribe<ApplicationPauseSignal>(this.OnApplicationPauseHandler);

            //AOA
            this.StartLoadingAOATime = DateTime.Now;
            //Pause can show AOA
            this.signalBus.Subscribe<InterstitialAdDisplayedSignal>(this.OnInterstitialAdDisplayedHandler);
            this.signalBus.Subscribe<RewardedAdDisplayedSignal>(this.ShownAdInDifferentProcessHandler);
            this.signalBus.Subscribe<OnStartDoingIAPSignal>(this.OnStartDoingIAPHandler);

            //Resume can show AOA
            this.signalBus.Subscribe<InterstitialAdClosedSignal>(this.CloseAdInDifferentProcessHandler);
            this.signalBus.Subscribe<InterstitialAdDisplayedFailedSignal>(this.CloseAdInDifferentProcessHandler);
            this.signalBus.Subscribe<RewardedAdClosedSignal>(this.CloseAdInDifferentProcessHandler);
            this.signalBus.Subscribe<RewardedAdShowFailedSignal>(this.CloseAdInDifferentProcessHandler);
            this.signalBus.Subscribe<RewardInterstitialAdClosedSignal>(this.CloseAdInDifferentProcessHandler);
            this.signalBus.Subscribe<OnIAPPurchaseSuccessSignal>(this.CloseAdInDifferentProcessHandler);
            this.signalBus.Subscribe<OnIAPPurchaseFailedSignal>(this.CloseAdInDifferentProcessHandler);

            //MREC
            this.signalBus.Subscribe<ScreenShowSignal>(this.OnScreenShow);
            this.signalBus.Subscribe<ScreenCloseSignal>(this.OnScreenClose);
            this.signalBus.Subscribe<MRecAdLoadedSignal>(this.OnMRECLoaded);
            this.signalBus.Subscribe<MRecAdDisplayedSignal>(this.OnMRECDisplayed);
            this.signalBus.Subscribe<MRecAdDismissedSignal>(this.OnMRECDismissed);

            //Collapsible
            this.signalBus.Subscribe<CollapsibleBannerAdLoadFailedSignal>(this.OnCollapsibleBannerLoadFailed);
            this.signalBus.Subscribe<CollapsibleBannerAdLoadedSignal>(this.OnCollapsibleBannerLoaded);
        }


        #region banner

        public BannerLoadStrategy BannerLoadStrategy => this.thirdPartiesConfig.AdSettings.BannerLoadStrategy;

        public virtual async void ShowBannerAd(BannerAdsPosition bannerAdsPosition = BannerAdsPosition.Bottom, int width = 320, int height = 50)
        {
            if (this.adServices.IsRemoveAds() || !this.adServicesConfig.EnableBannerAd)
            {
                return;
            }

            this.IsShowBannerAd = true;
            await UniTask.WaitUntil(() => this.adServices.IsAdsInitialized());

            if (this.IsShowBannerAd)
            {
                if (this.adServicesConfig.EnableCollapsibleBanner)
                {
                    this.InternalShowCollapsibleBannerAd();
                }
                else
                {
                    this.InternalShowMediationBannerAd(bannerAdsPosition, width, height);
                }
            }
        }

        private void InternalShowMediationBannerAd(BannerAdsPosition bannerAdsPosition = BannerAdsPosition.Bottom, int width = 320, int height = 50)
        {
            this.adServices.ShowBannerAd(bannerAdsPosition, width, height);
        }

        private void InternalShowCollapsibleBannerAd() { this.collapsibleBannerAd.ShowCollapsibleBannerAd(); }

        public virtual void HideBannerAd()
        {
            this.IsShowBannerAd = false;
            if (this.adServicesConfig.EnableCollapsibleBanner)
            {
                this.InternalHideCollapsibleBannerAd();
            }
            else
            {
                this.InternalHideMediationBannerAd();
            }
        }

        private void InternalHideMediationBannerAd() { this.adServices.HideBannedAd(); }

        private void InternalHideCollapsibleBannerAd()
        {
            // this.collapsibleBannerAd.HideCollapsibleBannerAd(); TODO uncomment when update collapsible
            this.collapsibleBannerAd.DestroyCollapsibleBannerAd();
        }

        private void OnCollapsibleBannerLoaded() { this.InternalHideMediationBannerAd(); }

        private void OnCollapsibleBannerLoadFailed()
        {
            this.InternalHideCollapsibleBannerAd();
            this.InternalShowMediationBannerAd();
        }

        #endregion

        private void OnInterstitialAdDisplayedHandler()
        {
            this.totalInterstitialAdsShowedInSession++;
            this.ShownAdInDifferentProcessHandler();
        }

        private void OnStartDoingIAPHandler() { this.IsResumedFromAdsOrIAP = true; }

        private async void CloseAdInDifferentProcessHandler()
        {
            await UniTask.Delay(TimeSpan.FromSeconds(1));
            this.IsResumedFromAdsOrIAP = false;
        }

        private void ShownAdInDifferentProcessHandler() { this.IsResumedFromAdsOrIAP = true; }

        private bool IsFiredFirstOpenEligibleSignal = false;

        private void CheckShowFirstOpen()
        {
            if (!this.IsFiredFirstOpenEligibleSignal)
            {
                this.signalBus.Fire(new AppOpenEligibleSignal(""));
                this.IsFiredFirstOpenEligibleSignal = true;
            }

            var totalLoadingTime = (DateTime.Now - this.StartLoadingAOATime).TotalSeconds;

            if (totalLoadingTime <= this.LoadingTimeToShowAOA)
            {
                this.ShowAOAAdsIfAvailable(false);
            }
            else
            {
                this.logService.Log($"AOA loading time for first open over the threshold {totalLoadingTime} > {this.LoadingTimeToShowAOA}!");
                this.IsCheckedShowFirstOpen = true; //prevent check AOA for first open again
            }
        }

        public double LoadingTimeToShowAOA => this.thirdPartiesConfig.AdSettings.AOAThreshHold;

        private void ShowAOAAdsIfAvailable(bool isFireEligibleSignal = true)
        {
            if (!this.adServicesConfig.EnableAOAAd) return;

            if (isFireEligibleSignal)
            {
                this.signalBus.Fire(new AppOpenEligibleSignal(""));
            }

            if (this.aoaAdServices.Any(aoaService => aoaService.IsAOAReady()))
            {
                this.signalBus.Fire(new AppOpenCalledSignal(""));
                this.aoaAdServices.First(aoaService => aoaService.IsAOAReady()).ShowAOAAds();
                this.IsCheckedShowFirstOpen = true;
                this.IsOpenedAOAFirstOpen   = true;
            }
        }

        private void OnApplicationPauseHandler(ApplicationPauseSignal obj)
        {
            if (obj.PauseStatus)
            {
                this.StartBackgroundTime = DateTime.Now;
                return;
            }

            var totalBackgroundSeconds = (DateTime.Now - this.StartBackgroundTime).TotalSeconds;
            if (totalBackgroundSeconds < this.adServicesConfig.MinPauseSecondToShowAoaAd)
            {
                this.logService.Log($"AOA background time: {totalBackgroundSeconds}");
                return;
            }

            // if (!this.config.OpenAOAAfterResuming) return;

            if (this.IsResumedFromAdsOrIAP)
            {
                return;
            }

            if (!this.IsRemovedAds)
            {
                this.ShowAOAAdsIfAvailable();
            }
        }

        private void OnBannerAdPresented(BannerAdPresentedSignal obj)
        {
            if (this.adServices.IsRemoveAds())
            {
                this.adServices.DestroyBannerAd();
            }
            else if (!this.IsShowBannerAd)
            {
                this.adServices.HideBannedAd();
            }
        }

        private void OnInterstitialDisplayedFailedHandler(InterstitialAdDisplayedFailedSignal obj) { this.DoOnInterstitialFinishedAction(false); }

        private void OnInterstitialAdClosedHandler()
        {
            this.DoOnInterstitialFinishedAction(true);
            this.totalNoAdsPlayingTime = 0;
        }

        private void DoOnInterstitialFinishedAction(bool isShowSuccess)
        {
            this.onInterstitialFinishedAction?.Invoke(isShowSuccess);
            this.onInterstitialFinishedAction = null;
        }

        #region InterstitialAd

        public virtual bool IsInterstitialAdReady(string place) { return this.adServices.IsInterstitialAdReady(place) || this.backFillAdsService.IsInterstitialAdReady(place); }

        private int FirstInterstitialAdsDelayTime =>
            this.gameSessionDataController.OpenTime > 1 ? this.adServicesConfig.DelayFirstInterNewSession : this.adServicesConfig.DelayFirstInterstitialAdInterval;

        private bool IsInterstitialAdEnable(string placement)
        {
            if (!this.adServicesConfig.EnableInterstitialAd) return false;
            return this.adServicesConfig.InterstitialAdActivePlacements.Contains("")
                   || this.adServicesConfig.InterstitialAdActivePlacements.Contains(placement);
        }

        public bool CanShowInterstitialAd(string place, bool force = false)
        {
            var isInterstitialAdEnable = this.IsInterstitialAdEnable(place);
            this.logService.Log(
                $"onelog: ShowInterstitialAd1 {place} force {force} this.adServicesConfig.EnableInterstitialAd {isInterstitialAdEnable} this.levelDataController.CurrentLevel {this.levelDataController.CurrentLevel} this.adServicesConfig.InterstitialAdStartLevel {this.adServicesConfig.InterstitialAdStartLevel}");
            if (this.adServices.IsRemoveAds() || !isInterstitialAdEnable || this.levelDataController.CurrentLevel < this.adServicesConfig.InterstitialAdStartLevel)
            {
                return false;
            }

            this.logService.Log(
                $"onelog: ShowInterstitialAd2 {place} force {force} check1 {this.totalNoAdsPlayingTime < this.adServicesConfig.InterstitialAdInterval} check2 {this.totalNoAdsPlayingTime < this.FirstInterstitialAdsDelayTime}");
            if ((this.totalNoAdsPlayingTime < this.adServicesConfig.InterstitialAdInterval ||
                 (this.totalInterstitialAdsShowedInSession == 0 && this.totalNoAdsPlayingTime < this.FirstInterstitialAdsDelayTime)) && !force)
            {
                this.logService.Warning("InterstitialAd was not passed interval");

                return false;
            }

            return true;
        }

        public virtual bool ShowInterstitialAd(string place, Action<bool> onShowInterstitialFinished, bool force = false)
        {
            if (!this.CanShowInterstitialAd(place, force))
            {
                onShowInterstitialFinished?.Invoke(false);
                return false;
            }

            this.signalBus.Fire(new InterstitialAdEligibleSignal(place));

            this.logService.Log($"onelog: ShowInterstitialAd3 {place} check1 {this.adServices.IsInterstitialAdReady(place)} check2 {this.backFillAdsService.IsInterstitialAdReady(place)}");
            if (!this.adServices.IsInterstitialAdReady(place))
            {
                if (!this.backFillAdsService.IsInterstitialAdReady(place))
                {
                    this.logService.Warning("InterstitialAd was not loaded");

                    onShowInterstitialFinished?.Invoke(false);
                    return false;
                }

                if (this.thirdPartiesConfig.AdSettings.EnableBreakAds)
                {
                    ShowDelayInter(() =>
                    {
                        InternalShowInterstitial();
                        this.backFillAdsService.ShowInterstitialAd(place);
                    }).Forget();
                }
                else
                {
                    InternalShowInterstitial();
                    this.backFillAdsService.ShowInterstitialAd(place);
                }

                return true;
            }

            if (this.thirdPartiesConfig.AdSettings.EnableBreakAds)
            {
                ShowDelayInter(() =>
                {
                    InternalShowInterstitial();
                    this.adServices.ShowInterstitialAd(place);
                }).Forget();
            }
            else
            {
                InternalShowInterstitial();
                this.adServices.ShowInterstitialAd(place);
            }

            return true;

            async UniTaskVoid ShowDelayInter(Action action)
            {
                await this.screenManager.OpenScreen<BreakAdsPopupPresenter>();
                await UniTask.Delay(TimeSpan.FromSeconds(0.5f), DelayType.UnscaledDeltaTime);
                action.Invoke();
            }

            void InternalShowInterstitial()
            {
                this.logService.Log($"onelog: ShowInterstitialAd4 {place}");
                this.totalNoAdsPlayingTime = 0;
                this.signalBus.Fire(new InterstitialAdCalledSignal(place));
                this.uiTemplateAdsController.UpdateWatchedInterstitialAds();
                this.IsResumedFromAdsOrIAP        = true;
                this.onInterstitialFinishedAction = onShowInterstitialFinished;
            }
        }

        public virtual void LoadInterstitialAd(string place) { this.signalBus.Fire(new InterstitialAdDownloadedSignal(place)); }

        #endregion

        #region RewardAd

        public virtual void ShowRewardedAd(string place, Action onComplete, Action onFail = null)
        {
            if (!this.adServicesConfig.EnableRewardedAd)
            {
                onComplete?.Invoke();
                return;
            }

            this.signalBus.Fire(new RewardedAdEligibleSignal(place));

            if (!this.adServices.IsRewardedAdReady(place))
            {
                this.logService.Warning("Rewarded was not loaded");
                onFail?.Invoke();
                this.toastController.ShowToast("There is no Ads!");

                return;
            }

            this.signalBus.Fire(new RewardedAdCalledSignal(place));
            this.uiTemplateAdsController.UpdateWatchedRewardedAds();
            this.IsResumedFromAdsOrIAP = true;
            this.adServices.ShowRewardedAd(place, OnRewardedAdCompleted, onFail);
            return;

            void OnRewardedAdCompleted()
            {
                onComplete?.Invoke();

                if (this.adServicesConfig.ResetInterAdIntervalAfterRewardAd)
                {
                    this.totalNoAdsPlayingTime = 0;
                }
            }
        }

        public virtual bool IsRewardedAdReady(string place) { return this.adServices.IsRewardedAdReady(place); }

        public virtual void RewardedAdOffer(string place) { this.signalBus.Fire(new RewardedAdOfferSignal(place)); }

        #endregion

        public virtual void ShowMREC<TPresenter>(AdViewPosition adViewPosition) where TPresenter : IScreenPresenter
        {
            if (this.adServices.IsRemoveAds() || !this.adServicesConfig.EnableMRECAd) return;

            var mrecAdService = this.mrecAdServices.FirstOrDefault(service => service.IsMRECReady(adViewPosition));

            if (mrecAdService != null)
            {
                this.AddScreenCanShowMREC(typeof(TPresenter));
                mrecAdService.ShowMREC(adViewPosition);
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
            }
        }

        private void OnRemoveAdsComplete()
        {
            this.HideAllMREC();

            this.collapsibleBannerAd.DestroyCollapsibleBannerAd();
            this.adServices.DestroyBannerAd();
        }

        public bool IsRemovedAds => this.adServices.IsRemoveAds();

        public void Tick()
        {
            this.totalNoAdsPlayingTime += Time.unscaledDeltaTime;

            if (!this.IsCheckedShowFirstOpen && this.gameSessionDataController.OpenTime >= this.adServicesConfig.AOAStartSession)
            {
                this.CheckShowFirstOpen();
            }
        }

        #region Auto Hide MREC

        private HashSet<Type> screenCanShowMREC = new();

        private void OnScreenShow(ScreenShowSignal signal)
        {
            // Work around to dont show banner 2 time in loading screen (we already show by the banner strategy in loading screen)
            if (!this.IsCheckFirstScreenShow)
            {
                this.IsCheckFirstScreenShow = true;
                return;
            }

            if (this.screenCanShowMREC.Contains(signal.ScreenPresenter.GetType()))
            {
                return;
            }

            this.HideAllMREC();
        }

        private void OnScreenClose(ScreenCloseSignal signal)
        {
            if (!this.screenCanShowMREC.Contains(signal.ScreenPresenter.GetType()))
            {
#if THEONE_COLLAPSIBLE_BANNER
                if (!this.thirdPartiesConfig.AdSettings.CollapsibleRefreshOnScreenShow) return;
                this.ShowBannerAd();
#endif
                return;
            }

            this.HideAllMREC();
        }

        private void OnMRECLoaded() { this.CheckCurrentScreenCanShowMREC(); }

        private void OnMRECDisplayed() { this.HideBannerAd(); }

        private void OnMRECDismissed() { this.ShowBannerAd(); }

        private void AddScreenCanShowMREC(Type screenType)
        {
            if (this.screenCanShowMREC.Contains(screenType))
            {
                this.logService.LogWithColor($"Screen: {screenType.Name} contained, can't add to collection!", Color.red);
                return;
            }

            this.screenCanShowMREC.Add(screenType);
        }

        private void CheckCurrentScreenCanShowMREC()
        {
            if (this.screenManager == null) return;
            if (this.screenManager.CurrentActiveScreen == null)
            {
                this.HideAllMREC();
                return;
            }

            if (this.screenCanShowMREC.Contains(this.screenManager.CurrentActiveScreen.Value.GetType())) return;
            this.HideAllMREC();
        }

        private void HideAllMREC()
        {
            foreach (var mrecAdService in this.mrecAdServices)
            {
                mrecAdService.HideAllMREC();
            }
        }

        #endregion

        public void RemoveAds()
        {
            this.adServices.RemoveAds();
            this.OnRemoveAdsComplete();
            this.signalBus.Fire<OnRemoveAdsSucceedSignal>();
        }
    }
}