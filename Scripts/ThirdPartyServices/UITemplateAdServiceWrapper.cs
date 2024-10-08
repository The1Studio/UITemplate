namespace TheOneStudio.UITemplate.UITemplate.Scripts.ThirdPartyServices
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using Core.AdsServices;
    using Core.AdsServices.CollapsibleBanner;
    using Core.AdsServices.Signals;
    using Cysharp.Threading.Tasks;
    using GameFoundation.DI;
    using GameFoundation.Scripts.UIModule.ScreenFlow.BaseScreen.Presenter;
    using GameFoundation.Scripts.UIModule.ScreenFlow.Managers;
    using GameFoundation.Scripts.UIModule.ScreenFlow.Signals;
    using GameFoundation.Scripts.Utilities.ApplicationServices;
    using GameFoundation.Scripts.Utilities.LogService;
    using GameFoundation.Signals;
    using R3;
    using ServiceImplementation.Configs;
    using ServiceImplementation.Configs.Ads;
    using ServiceImplementation.IAPServices.Signals;
    using TheOneStudio.UITemplate.UITemplate.Models.Controllers;
    using TheOneStudio.UITemplate.UITemplate.Scenes.Loading;
    using TheOneStudio.UITemplate.UITemplate.Services.BreakAds;
    using TheOneStudio.UITemplate.UITemplate.Services.Permissions.Signals;
    using TheOneStudio.UITemplate.UITemplate.Services.Toast;
    using TheOneStudio.UITemplate.UITemplate.Signals;
    using UnityEngine;
    using UnityEngine.Scripting;
    #if ADMOB
    using ServiceImplementation.AdsServices.EasyMobile;
    #endif

    public class UITemplateAdServiceWrapper : IInitializable, ITickable
    {
        #region inject

        private readonly IReadOnlyList<IAdServices>          adServices;
        private readonly IReadOnlyList<IMRECAdService>       mrecAdServices;
        private readonly UITemplateAdsController             uiTemplateAdsController;
        private readonly UITemplateGameSessionDataController gameSessionDataController;
        private readonly IReadOnlyList<IAOAAdService>        aoaAdServices;
        private readonly ToastController                     toastController;
        private readonly UITemplateLevelDataController       levelDataController;
        private readonly ThirdPartiesConfig                  thirdPartiesConfig;
        private readonly IScreenManager                      screenManager;
        private readonly ICollapsibleBannerAd                collapsibleBannerAd;
        private readonly ILogService                         logService;
        private readonly AdServicesConfig                    adServicesConfig;
        private readonly SignalBus                           signalBus;
        private readonly IAdServices                         bannerAdService;
        private readonly IReadOnlyCollection<IAdServices>    interstitialAdServices;
        private readonly IReadOnlyCollection<IAdServices>    rewardedAdServices;

        #endregion

        //Interstitial
        private float        totalNoAdsPlayingTime;
        private Action<bool> onInterstitialFinishedAction;
        private int          totalInterstitialAdsShowedInSession;

        //Banner
        private bool                    IsShowBannerAd                        { get; set; }
        private bool                    IsShowMRECAd                          { get; set; }
        private bool                    IsCheckFirstScreenShow                { get; set; }
        private DateTime                LastCollapsibleBannerChangeGuid       { get; set; } = DateTime.MinValue;
        private bool                    PreviousCollapsibleBannerAdLoadedFail { get; set; }
        private bool                    IsRefreshingCollapsible               { get; set; }
        private CancellationTokenSource RefreshCollapsibleCts                 { get; set; }

        //AOA
        private DateTime StartLoadingAOATime          { get; set; }
        private DateTime StartBackgroundTime          { get; set; }
        private bool     IsResumedFromAnotherServices { get; set; } // after Ads, IAP, permission, login, etc.
        private bool     IsCheckedShowFirstOpen       { get; set; } = false;
        public  bool     IsOpenedAOAFirstOpen         { get; private set; }

        [Preserve]
        public UITemplateAdServiceWrapper(
            ILogService                         logService,
            AdServicesConfig                    adServicesConfig,
            SignalBus                           signalBus,
            IEnumerable<IAdServices>            adServices,
            IEnumerable<IMRECAdService>         mrecAdServices,
            UITemplateAdsController             uiTemplateAdsController,
            UITemplateGameSessionDataController gameSessionDataController,
            IEnumerable<IAOAAdService>          aoaAdServices,
            ToastController                     toastController,
            UITemplateLevelDataController       levelDataController,
            ThirdPartiesConfig                  thirdPartiesConfig,
            IScreenManager                      screenManager,
            ICollapsibleBannerAd                collapsibleBannerAd,
            IEnumerable<AdServiceOrder>         adServiceOrders
        )
        {
            this.adServices                = adServices.ToArray();
            this.mrecAdServices            = mrecAdServices.ToArray();
            this.uiTemplateAdsController   = uiTemplateAdsController;
            this.gameSessionDataController = gameSessionDataController;
            this.aoaAdServices             = aoaAdServices.ToArray();
            this.toastController           = toastController;
            this.levelDataController       = levelDataController;
            this.thirdPartiesConfig        = thirdPartiesConfig;
            this.screenManager             = screenManager;
            this.collapsibleBannerAd       = collapsibleBannerAd;
            this.logService                = logService;
            this.adServicesConfig          = adServicesConfig;
            this.signalBus                 = signalBus;
            var adServiceOrdersDict = adServiceOrders.ToDictionary(order => (order.ServiceType, order.AdType), order => order.Order);
            this.bannerAdService        = this.adServices.OrderBy(adService => adServiceOrdersDict.GetValueOrDefault((adService.GetType(), AdType.Banner))).First();
            this.interstitialAdServices = this.adServices.OrderBy(adService => adServiceOrdersDict.GetValueOrDefault((adService.GetType(), AdType.Interstitial))).ToArray();
            this.rewardedAdServices     = this.adServices.OrderBy(adService => adServiceOrdersDict.GetValueOrDefault((adService.GetType(), AdType.Rewarded))).ToArray();
        }

        public void Initialize()
        {
            this.signalBus.Subscribe<InterstitialAdClosedSignal>(this.OnInterstitialAdClosedHandler);
            this.signalBus.Subscribe<InterstitialAdDisplayedFailedSignal>(this.OnInterstitialDisplayedFailedHandler);
            this.signalBus.Subscribe<BannerAdPresentedSignal>(this.OnBannerAdPresented);
            this.signalBus.Subscribe<ApplicationPauseSignal>(this.OnApplicationPauseHandler);
            this.signalBus.Subscribe<CollapsibleBannerAdLoadFailedSignal>(this.OnCollapsibleBannerLoadFailed);

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
            this.screenManager.CurrentActiveScreen.Subscribe(this.OnScreenChanged);

            //Permission
            this.signalBus.Subscribe<OnRequestPermissionStartSignal>(this.ShownAdInDifferentProcessHandler);
            this.signalBus.Subscribe<OnRequestPermissionCompleteSignal>(this.CloseAdInDifferentProcessHandler);

            //MREC
            this.signalBus.Subscribe<ScreenShowSignal>(this.OnScreenShow);
            this.signalBus.Subscribe<ScreenCloseSignal>(this.OnScreenClose);
            this.signalBus.Subscribe<MRecAdLoadedSignal>(this.OnMRECLoaded);

            this.signalBus.Subscribe<CollapsibleBannerAdLoadedSignal>(this.OnBannerLoaded);
            this.signalBus.Subscribe<BannerAdLoadedSignal>(this.OnBannerLoaded);
        }

        #region banner

        public BannerLoadStrategy BannerLoadStrategy => this.thirdPartiesConfig.AdSettings.BannerLoadStrategy;

        public virtual async void ShowBannerAd(int width = 320, int height = 50)
        {
            if (this.IsRemovedAds) return;

            this.IsShowBannerAd = true;

            await UniTask.WaitUntil(() => this.bannerAdService.IsAdsInitialized());

            this.logService.Log($"onelog: ShowBannerAd IsCurrentScreenCanShowMREC {this.IsCurrentScreenCanShowMREC()} this.adServicesConfig.EnableBannerAd {this.adServicesConfig.EnableBannerAd}");
            if (this.IsCurrentScreenCanShowMREC() || !this.adServicesConfig.EnableBannerAd) return;
            this.logService.Log($"onelog: ShowBannerAd IsShowBannerAd {this.IsShowBannerAd}");
            if (this.IsShowBannerAd)
            {
                this.logService.Log($"onelog: ShowBannerAd EnableCollapsibleBanner {this.adServicesConfig.EnableCollapsibleBanner}, PreviousCollapsibleBannerAdLoadedFail {this.PreviousCollapsibleBannerAdLoadedFail}");
                if (this.adServicesConfig.EnableCollapsibleBanner && !this.PreviousCollapsibleBannerAdLoadedFail)
                {
                    var useNewGuid = this.IsRefreshingCollapsible
                        ? this.adServicesConfig.CollapsibleBannerExpandOnRefreshEnabled
                        : (DateTime.Now - this.LastCollapsibleBannerChangeGuid).TotalSeconds >= this.adServicesConfig.CollapsibleBannerExpandOnRefreshInterval;
                    if (useNewGuid)
                    {
                        this.LastCollapsibleBannerChangeGuid = DateTime.Now;
                    }

                    this.InternalHideMediationBannerAd();
                    this.collapsibleBannerAd.ShowCollapsibleBannerAd(useNewGuid, this.thirdPartiesConfig.AdSettings.BannerPosition);
                    this.logService.Log($"onelog: ShowCollapsibleBannerAd refreshing: {this.IsRefreshingCollapsible}, expandOnRefresh: {this.adServicesConfig.CollapsibleBannerExpandOnRefreshEnabled}, useNewGuid: {useNewGuid}");
                    this.IsRefreshingCollapsible = false;
                    this.ScheduleRefreshCollapsible();
                }
                else
                {
                    this.InternalHideCollapsibleBannerAd();
                    this.InternalShowMediationBannerAd(this.thirdPartiesConfig.AdSettings.BannerPosition, width, height);
                    this.logService.Log("onelog: InternalShowMediationBannerAd");
                }

                this.PreviousCollapsibleBannerAdLoadedFail = false;
                this.signalBus.Fire(new UITemplateOnUpdateBannerStateSignal(true));
            }
        }

        private void ScheduleRefreshCollapsible()
        {
            this.RefreshCollapsibleCts?.Cancel();
            this.RefreshCollapsibleCts?.Dispose();
            this.RefreshCollapsibleCts = null;
            if (this.adServicesConfig.CollapsibleBannerADInterval <= 0) return;
            if (!this.adServicesConfig.CollapsibleBannerAutoRefreshEnabled) return;
            UniTask.WaitForSeconds(
                this.adServicesConfig.CollapsibleBannerADInterval,
                ignoreTimeScale: true,
                cancellationToken: (this.RefreshCollapsibleCts = new()).Token
            ).ContinueWith(() =>
            {
                this.IsRefreshingCollapsible = true;
                this.ShowBannerAd();
            }).Forget();
        }

        private void InternalShowMediationBannerAd(BannerAdsPosition bannerAdsPosition = BannerAdsPosition.Bottom, int width = 320, int height = 50)
        {
            this.bannerAdService.ShowBannerAd(bannerAdsPosition, width, height);
        }

        public virtual void HideBannerAd()
        {
            this.IsShowBannerAd = false;
            this.InternalHideCollapsibleBannerAd();
            this.InternalHideMediationBannerAd();
            this.signalBus.Fire(new UITemplateOnUpdateBannerStateSignal(false));
            this.logService.Log("onelog: HideBannerAd");
        }

        private void InternalHideMediationBannerAd() { this.bannerAdService.HideBannedAd(); }

        private void InternalHideCollapsibleBannerAd()
        {
            this.RefreshCollapsibleCts?.Cancel();
            this.RefreshCollapsibleCts?.Dispose();
            this.RefreshCollapsibleCts = null;
            // this.collapsibleBannerAd.HideCollapsibleBannerAd(); TODO uncomment when update collapsible
            this.collapsibleBannerAd.DestroyCollapsibleBannerAd();
        }

        private void OnBannerLoaded()
        {
            if (this.IsCurrentScreenCanShowMREC())
            {
                this.HideBannerAd();
            }
        }

        private void OnScreenChanged(IScreenPresenter screenPresenter)
        {
            if (screenPresenter == null) return;

            #if THEONE_COLLAPSIBLE_BANNER
            if (!this.thirdPartiesConfig.AdSettings.CollapsibleRefreshOnScreenShow) return;
            if (this.thirdPartiesConfig.AdSettings.CollapsibleIgnoreRefreshOnScreens.Contains(screenPresenter.GetType().Name)) return;
            if (this.BannerLoadStrategy == BannerLoadStrategy.AfterLoading && screenPresenter is UITemplateLoadingScreenPresenter) return;
            this.ShowBannerAd();
            #endif
        }

        private void OnCollapsibleBannerLoadFailed()
        {
            // if collapsible banner load fail, we will show mediation banner
            if (!this.adServicesConfig.EnableCollapsibleBannerFallback) return;
            this.PreviousCollapsibleBannerAdLoadedFail = true;
            this.ShowBannerAd();
        }

        #endregion

        private void OnInterstitialAdDisplayedHandler()
        {
            this.totalInterstitialAdsShowedInSession++;
            this.ShownAdInDifferentProcessHandler();
        }

        private void OnStartDoingIAPHandler() { this.IsResumedFromAnotherServices = true; }

        private async void CloseAdInDifferentProcessHandler()
        {
            await UniTask.Delay(TimeSpan.FromSeconds(1));
            this.IsResumedFromAnotherServices = false;
        }

        private void ShownAdInDifferentProcessHandler() { this.IsResumedFromAnotherServices = true; }

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

        public double LoadingTimeToShowAOA => this.adServicesConfig.AOALoadingThreshold;

        private void ShowAOAAdsIfAvailable(bool isFireEligibleSignal = true)
        {
            if (!this.adServicesConfig.EnableAOAAd) return;
            if (this.IsRemovedAds) return;

            if (isFireEligibleSignal)
            {
                this.signalBus.Fire(new AppOpenEligibleSignal(""));
            }

            var typeToAvailable = string.Join(" | ", this.aoaAdServices.Select(aoa => aoa.GetType().Name + " isReady: " + aoa.IsAOAReady()));
            this.logService.Log($"onelog: AdServiceWrapper: ShowAOAAdsIfAvailable: useAdmob: {this.adServicesConfig.UseAoaAdmob} | {typeToAvailable}");
            foreach (var aoa in this.aoaAdServices.Where(aoaService => aoaService.IsAOAReady()))
            {
                if ((this.adServicesConfig.UseAoaAdmob
                        #if ADMOB
                        && aoa is AdMobWrapper
                        #endif
                    )
                    || (!this.adServicesConfig.UseAoaAdmob
                        #if ADMOB
                        && aoa is not AdMobWrapper
                        #endif
                    ))
                {
                    this.signalBus.Fire(new AppOpenCalledSignal(""));
                    aoa.ShowAOAAds();
                    this.IsCheckedShowFirstOpen = true;
                    this.IsOpenedAOAFirstOpen   = true;
                    return;
                }
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
            
            if (this.IsResumedFromAnotherServices)
            {
                return;
            }

            this.ShowAOAAdsIfAvailable();
        }

        private void OnBannerAdPresented(BannerAdPresentedSignal obj)
        {
            if (this.IsRemovedAds)
            {
                this.bannerAdService.DestroyBannerAd();
            }
            else if (!this.IsShowBannerAd)
            {
                this.bannerAdService.HideBannedAd();
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

        public virtual bool IsInterstitialAdReady(string place) => this.adServices.Any(adService => adService.IsInterstitialAdReady(place));

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
            if (this.IsRemovedAds || !isInterstitialAdEnable || this.levelDataController.CurrentLevel < this.adServicesConfig.InterstitialAdStartLevel)
            {
                return false;
            }

            var interstitialAdInterval = this.adServicesConfig.InterstitialAdInterval;
            if (this.thirdPartiesConfig.AdSettings.CustomInterstitialCappingTime.TryGetValue(place, out var cappingTime) && cappingTime.GetCappingTime > -1)
            {
                interstitialAdInterval = cappingTime.GetCappingTime;
            }

            this.logService.Log(
                $"onelog: ShowInterstitialAd2 {place} force {force} check1 {this.totalNoAdsPlayingTime < interstitialAdInterval} check2 {this.totalNoAdsPlayingTime < this.FirstInterstitialAdsDelayTime}");
            if ((this.totalNoAdsPlayingTime < interstitialAdInterval || (this.totalInterstitialAdsShowedInSession == 0 && this.totalNoAdsPlayingTime < this.FirstInterstitialAdsDelayTime)) && !force)
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

            this.logService.Log($"onelog: ShowInterstitialAd {place} - {string.Join(", ", this.adServices.Select(adService => $"{adService.GetType().Name}: {adService.IsInterstitialAdReady(place)}"))}");

            if (this.interstitialAdServices.FirstOrDefault(adService => adService.IsInterstitialAdReady(place)) is not { } adService)
            {
                this.logService.Warning("InterstitialAd was not loaded");
                onShowInterstitialFinished?.Invoke(false);
                return false;
            }

            this.logService.Log($"onelog: ShowInterstitialAd {place} - {adService.GetType().Name}");

            if (this.thirdPartiesConfig.AdSettings.EnableBreakAds)
            {
                ShowDelayInter(InternalShowInterstitial).Forget();
            }
            else
            {
                InternalShowInterstitial();
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
                this.totalNoAdsPlayingTime = 0;
                var adInfo = new AdInfo(adService.AdPlatform, place, "Interstitial");
                this.signalBus.Fire(new InterstitialAdCalledSignal(place, adInfo));
                this.uiTemplateAdsController.UpdateWatchedInterstitialAds();
                this.IsResumedFromAnotherServices = true;
                this.onInterstitialFinishedAction = onShowInterstitialFinished;
                adService.ShowInterstitialAd(place);
            }
        }

        #endregion

        #region RewardAd

        public virtual void ShowRewardedAd(string place, Action onComplete, Action onFail = null)
        {
            if (!this.adServicesConfig.EnableRewardedAd || this.adServicesConfig.RewardedAdFreePlacements.Contains(place))
            {
                onComplete?.Invoke();
                return;
            }

            this.signalBus.Fire(new RewardedAdEligibleSignal(place));

            this.logService.Log($"onelog: ShowRewardedAd {place} - {string.Join(", ", this.adServices.Select(adService => $"{adService.GetType().Name}: {adService.IsRewardedAdReady(place)}"))}");

            if (this.rewardedAdServices.FirstOrDefault(adService => adService.IsRewardedAdReady(place)) is not { } adService)
            {
                this.logService.Warning("Rewarded was not loaded");
                onFail?.Invoke();
                this.toastController.ShowToast("There is no Ads!");
                return;
            }

            this.logService.Log($"onelog: ShowRewardedAd {place} - {adService.GetType().Name}");

            var adInfo = new AdInfo(adService.AdPlatform, place, "Rewarded");
            this.signalBus.Fire(new RewardedAdCalledSignal(place, adInfo));
            this.uiTemplateAdsController.UpdateWatchedRewardedAds();
            this.IsResumedFromAnotherServices = true;
            adService.ShowRewardedAd(place, OnRewardedAdCompleted, onFail);
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

        public virtual bool IsRewardedAdReady(string place) => this.adServices.Any(adService => adService.IsRewardedAdReady(place));

        public virtual void RewardedAdOffer(string place) { this.signalBus.Fire(new RewardedAdOfferSignal(place)); }

        #endregion

        public virtual void ShowMREC<TPresenter>(AdViewPosition adViewPosition) where TPresenter : IScreenPresenter
        {
            if (this.IsRemovedAds || !this.adServicesConfig.EnableMRECAd) return;

            var mrecAdService = this.mrecAdServices.FirstOrDefault(service => service.IsMRECReady(adViewPosition));

            if (mrecAdService != null)
            {
                this.AddScreenCanShowMREC(typeof(TPresenter));
                mrecAdService.ShowMREC(adViewPosition);
                this.IsShowMRECAd = true;
                this.logService.Log($"onelog: ShowMREC {adViewPosition}");
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
                this.logService.Log($"onelog: HideMREC {adViewPosition}");
            }
        }

        private void OnRemoveAdsComplete()
        {
            this.HideAllMREC();

            this.collapsibleBannerAd.DestroyCollapsibleBannerAd();
            this.bannerAdService.DestroyBannerAd();
        }

        public bool IsRemovedAds => this.adServices.Any(adService => adService.IsRemoveAds());

        public void Tick()
        {
            // Problem: Time.unscaledDeltaTime is still incrementing when the game is paused, so if we somehow pause the game, the capping time will be increased
            // Solution: Time.unscaledDeltaTime is count from the last frame to current frame, so if the time between 2 frames is too long, we will not increase the capping time
            // If EnableInterCappingTimeFocus is set to false, we will keep the existing capping time behavior
            // If EnableInterCappingTimeFocus is set to true, we will not increase the capping time while application is not focused
            if (!this.thirdPartiesConfig.AdSettings.EnableInterCappingTimeFocus || Time.unscaledDeltaTime < 1)
            {
                this.totalNoAdsPlayingTime += Time.unscaledDeltaTime;
            }
            
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
            if (!this.screenCanShowMREC.Contains(signal.ScreenPresenter.GetType())) return;

            this.HideAllMREC();
        }

        private void OnMRECLoaded()
        {
            if (!this.IsCurrentScreenCanShowMREC())
            {
                this.HideAllMREC();
            }
        }

        private void AddScreenCanShowMREC(Type screenType)
        {
            if (this.screenCanShowMREC.Contains(screenType))
            {
                this.logService.LogWithColor($"Screen: {screenType.Name} contained, can't add to collection!", Color.red);
                return;
            }

            this.screenCanShowMREC.Add(screenType);
        }

        private void HideAllMREC()
        {
            if (!this.IsShowMRECAd) return;
            if (!this.IsShowBannerAd) this.ShowBannerAd();
            foreach (var mrecAdService in this.mrecAdServices)
            {
                mrecAdService.HideAllMREC();
                this.IsShowMRECAd = false;
            }
        }

        private bool IsCurrentScreenCanShowMREC() => this.screenManager?.CurrentActiveScreen?.Value != null && this.screenCanShowMREC.Contains(this.screenManager.CurrentActiveScreen.Value.GetType());

        #endregion

        public void RemoveAds()
        {
            foreach (var adService in this.adServices)
            {
                adService.RemoveAds();
            }
            this.OnRemoveAdsComplete();
            this.signalBus.Fire<OnRemoveAdsSucceedSignal>();
        }
    }
}