namespace TheOneStudio.UITemplate.UITemplate.Scripts.ThirdPartyServices
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using Core.AdsServices;
    using Core.AdsServices.CollapsibleBanner;
    using Core.AdsServices.Native;
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
    using ServiceImplementation;
    using ServiceImplementation.AdsServices.AdMob.NativeOverlay;
    using ServiceImplementation.AdsServices.ConsentInformation;
    using ServiceImplementation.AdsServices.Signal;
    using ServiceImplementation.Configs;
    using ServiceImplementation.Configs.Ads;
    using ServiceImplementation.IAPServices.Signals;
    using TheOneStudio.UITemplate.UITemplate.Models.Controllers;
    using TheOneStudio.UITemplate.UITemplate.Scenes.Loading;
    using TheOneStudio.UITemplate.UITemplate.Scenes.Popups;
    using TheOneStudio.UITemplate.UITemplate.Services.BreakAds;
    using TheOneStudio.UITemplate.UITemplate.Services.Permissions.Signals;
    using TheOneStudio.UITemplate.UITemplate.Services.Toast;
    using TheOneStudio.UITemplate.UITemplate.Signals;
    using TheOneStudio.UITemplate.UITemplate.ThirdPartyServices.AnalyticEvents.CommonEvents;
    using UnityEngine;
    using UnityEngine.Scripting;
    #if ADMOB
    using ServiceImplementation.AdsServices.AdMob;
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
        private readonly IConsentInformation                 consentInformation;
        private readonly NativeOverlayWrapper                nativeOverlayWrapper;
        private readonly ILogService                         logService;
        private readonly AdServicesConfig                    adServicesConfig;
        private readonly SignalBus                           signalBus;
        private readonly IAdServices                         bannerAdService;
        private readonly IReadOnlyCollection<IAdServices>    interstitialAdServices;
        private readonly IReadOnlyCollection<IAdServices>    rewardedAdServices;
        private readonly IReadOnlyList<INativeAdsService>    nativeAdsServices;

        #endregion

        //Interstitial
        private float        totalNoAdsPlayingTime;
        private Action<bool> onInterstitialFinishedAction;
        private int          totalInterstitialAdsShowedInSession;

        //Banner
        private bool                    IsShowBannerAd                        { get; set; }
        private bool                    IsMediationBanner                     { get; set; }
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
        private bool     IsShowingAOA                 { get; set; }

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
            IEnumerable<AdServiceOrder>         adServiceOrders,
            IConsentInformation                 consentInformation,
            IEnumerable<INativeAdsService>      nativeAdsServices,
            NativeOverlayWrapper                nativeOverlayWrapper
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
            this.consentInformation        = consentInformation;
            this.nativeOverlayWrapper      = nativeOverlayWrapper;
            this.logService                = logService;
            this.adServicesConfig          = adServicesConfig;
            this.signalBus                 = signalBus;
            var adServiceOrdersDict = adServiceOrders.ToDictionary(order => (order.ServiceType, order.AdType), order => order.Order);
            this.bannerAdService        = this.adServices.OrderBy(adService => adServiceOrdersDict.GetValueOrDefault((adService.GetType(), AdType.Banner))).First();
            this.interstitialAdServices = this.adServices.OrderBy(adService => adServiceOrdersDict.GetValueOrDefault((adService.GetType(), AdType.Interstitial))).ToArray();
            this.rewardedAdServices     = this.adServices.OrderBy(adService => adServiceOrdersDict.GetValueOrDefault((adService.GetType(), AdType.Rewarded))).ToArray();
            this.nativeAdsServices       = nativeAdsServices.ToArray();
        }

        public void Initialize()
        {
            this.signalBus.Subscribe<InterstitialAdClosedSignal>(this.OnInterstitialAdClosedHandler);
            this.signalBus.Subscribe<InterstitialAdDisplayedFailedSignal>(this.OnInterstitialDisplayedFailedHandler);
            this.signalBus.Subscribe<BannerAdPresentedSignal>(this.OnBannerAdPresented);
            this.signalBus.Subscribe<ApplicationPauseSignal>(this.OnApplicationPauseHandler);
            this.signalBus.Subscribe<CollapsibleBannerAdLoadFailedSignal>(this.OnCollapsibleBannerLoadFailed);
            this.signalBus.Subscribe<CollapsibleBannerAdDismissedSignal>(this.OnCollapsibleBannerDismissed);

            //AOA
            this.StartLoadingAOATime = DateTime.Now;
            //Pause can show AOA
            this.signalBus.Subscribe<InterstitialAdDisplayedSignal>(this.OnInterstitialAdDisplayedHandler);
            this.signalBus.Subscribe<RewardedAdDisplayedSignal>(this.ShownAdInDifferentProcessHandler);
            this.signalBus.Subscribe<OnStartDoingIAPSignal>(this.OnStartDoingIAPHandler);
            this.signalBus.Subscribe<AppOpenFullScreenContentOpenedSignal>(this.OnOpenAOA);

            //Resume can show AOA
            this.signalBus.Subscribe<InterstitialAdClosedSignal>(this.CloseAdInDifferentProcessHandler);
            this.signalBus.Subscribe<InterstitialAdDisplayedFailedSignal>(this.CloseAdInDifferentProcessHandler);
            this.signalBus.Subscribe<RewardedAdClosedSignal>(this.CloseAdInDifferentProcessHandler);
            this.signalBus.Subscribe<RewardedAdShowFailedSignal>(this.CloseAdInDifferentProcessHandler);
            this.signalBus.Subscribe<RewardInterstitialAdClosedSignal>(this.CloseAdInDifferentProcessHandler);
            this.signalBus.Subscribe<OnIAPPurchaseSuccessSignal>(this.CloseAdInDifferentProcessHandler);
            this.signalBus.Subscribe<OnIAPPurchaseFailedSignal>(this.CloseAdInDifferentProcessHandler);
            this.signalBus.Subscribe<AppOpenFullScreenContentClosedSignal>(this.CloseAdInDifferentProcessHandler);
            this.screenManager.CurrentActiveScreen.Subscribe(this.OnScreenChanged);

            //Att
            this.signalBus.Subscribe<AttDisplayedSignal>(this.ShownAdInDifferentProcessHandler);
            this.signalBus.Subscribe<AttClosedSignal>(this.CloseAdInDifferentProcessHandler);

            //Permission
            this.signalBus.Subscribe<OnRequestPermissionStartSignal>(this.ShownAdInDifferentProcessHandler);
            this.signalBus.Subscribe<OnRequestPermissionCompleteSignal>(this.CloseAdInDifferentProcessHandler);
        }

        private void OnOpenAOA(AppOpenFullScreenContentOpenedSignal obj)
        {
            this.IsResumedFromAnotherServices = true;
        }

        #region banner

        public BannerLoadStrategy BannerLoadStrategy      => this.thirdPartiesConfig.AdSettings.BannerLoadStrategy;
        public bool               ShouldShowBannerAds() => !this.bannerAdService.IsRemoveAds() && this.adServicesConfig.EnableBannerAd;

        public virtual async void ShowBannerAd(int width = 320, int height = 50, bool forceShowMediation = false)
        {
            if (this.IsRemovedAds) return;

            this.IsShowBannerAd = true;

            await UniTask.WaitUntil(() => this.bannerAdService.IsAdsInitialized());

            this.logService.Log($"onelog: ShowBannerAd EnableBannerAd {this.adServicesConfig.EnableBannerAd}, IsShowBannerAd {this.IsShowBannerAd}");
            if (!this.adServicesConfig.EnableBannerAd) return;
            if (this.IsShowBannerAd)
            {
                this.IsMediationBanner = forceShowMediation || !this.adServicesConfig.EnableCollapsibleBanner || this.PreviousCollapsibleBannerAdLoadedFail;
                this.logService.Log($"onelog: ShowBannerAd EnableCollapsibleBanner {this.adServicesConfig.EnableCollapsibleBanner}, PreviousCollapsibleBannerAdLoadedFail {this.PreviousCollapsibleBannerAdLoadedFail}, IsMediationBanner {this.IsMediationBanner}");
                if (!this.IsMediationBanner)
                {
                    var useNewGuid = this.IsRefreshingCollapsible
                        ? this.adServicesConfig.CollapsibleBannerExpandOnRefreshEnabled
                        : (DateTime.Now - this.LastCollapsibleBannerChangeGuid).TotalSeconds >= this.adServicesConfig.CollapsibleBannerADInterval;
                    if (useNewGuid) this.LastCollapsibleBannerChangeGuid = DateTime.Now;

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
        private void OnCollapsibleBannerDismissed(CollapsibleBannerAdDismissedSignal signal)
        {
            this.ScheduleRefreshCollapsible();
        }
        private void ScheduleRefreshCollapsible()
        {
            this.RefreshCollapsibleCts?.Cancel();
            this.RefreshCollapsibleCts?.Dispose();
            this.RefreshCollapsibleCts = null;
            if (this.adServicesConfig.CollapsibleBannerExpandOnRefreshInterval <= 0) return;
            if (!this.adServicesConfig.CollapsibleBannerAutoRefreshEnabled) return;
            UniTask.WaitForSeconds(
                this.adServicesConfig.CollapsibleBannerExpandOnRefreshInterval,
                true,
                cancellationToken: (this.RefreshCollapsibleCts = new()).Token
            ).ContinueWith(() =>
            {
                this.IsRefreshingCollapsible = true;
                this.HideBannerAd();
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
            this.logService.Log("onelog: HideBannerAd");
        }

        private void InternalHideMediationBannerAd()
        {
            this.bannerAdService.HideBannedAd();
            this.signalBus.Fire(new UITemplateOnUpdateBannerStateSignal(false));
        }

        private void InternalHideCollapsibleBannerAd()
        {
            this.RefreshCollapsibleCts?.Cancel();
            this.RefreshCollapsibleCts?.Dispose();
            this.RefreshCollapsibleCts = null;
            // this.collapsibleBannerAd.HideCollapsibleBannerAd(); TODO uncomment when update collapsible
            this.collapsibleBannerAd.DestroyCollapsibleBannerAd();
            this.signalBus.Fire(new UITemplateOnUpdateBannerStateSignal(false));
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

        private void OnStartDoingIAPHandler()
        {
            this.IsResumedFromAnotherServices = true;
        }

        private async void CloseAdInDifferentProcessHandler()
        {
            await UniTask.Delay(TimeSpan.FromSeconds(1));
            this.IsResumedFromAnotherServices = false;
        }

        private void ShownAdInDifferentProcessHandler()
        {
            this.IsResumedFromAnotherServices = true;
        }

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
                this.ShowAOAAdsIfAvailable(true).Forget();
            }
            else
            {
                this.logService.Log($"AOA loading time for first open over the threshold {totalLoadingTime} > {this.LoadingTimeToShowAOA}!");
                this.IsCheckedShowFirstOpen = true; //prevent check AOA for first open again
            }
        }

        public double LoadingTimeToShowAOA => this.adServicesConfig.AOALoadingThreshold;

        protected virtual async UniTaskVoid ShowAOAAdsIfAvailable(bool isOpenAppAOA)
        {
            this.logService.Log($"onelog: AdServiceWrapper: ShowAOAAdsIfAvailable firstopen {isOpenAppAOA} IsRemovedAds {this.IsRemovedAds} EnableAOAAd {this.adServicesConfig.EnableAOAAd} TrackingComplete {AttHelper.IsRequestTrackingComplete()} IsIntersInsteadAoaResume {this.adServicesConfig.IsIntersInsteadAoaResume} ConsentCanRequestAds {this.consentInformation.CanRequestAds()} IsShowingAOA {this.IsShowingAOA}");

            if (this.IsShowingAOA) return;
            if (!this.adServicesConfig.EnableAOAAd) return;
            if (this.IsRemovedAds) return;
            if (!AttHelper.IsRequestTrackingComplete()) return;

            if (!this.consentInformation.CanRequestAds() && isOpenAppAOA)
            {
                this.IsShowingAOA = true;
                await UniTask.WaitUntil(() => !this.consentInformation.IsRequestingConsent());
                this.IsShowingAOA = false;
                if (!this.consentInformation.CanRequestAds() || this.screenManager.CurrentActiveScreen.Value is not UITemplateLoadingScreenPresenter) return;
            }

            #if BRAVESTARS
            if (isOpenAppAOA && !this.adServicesConfig.AoaFirstOpen && this.gameSessionDataController.OpenTime == 1) return;
            if (isOpenAppAOA && !this.adServicesConfig.AoaStartGame) return;
            #endif

            var placement = isOpenAppAOA ? AppOpenPlacement.FirstOpen.ToString() : AppOpenPlacement.ResumeApp.ToString();

            if (!isOpenAppAOA)
            {
                this.signalBus.Fire(new AppOpenEligibleSignal(placement)); // fire here instead of outside because we already fire the eligible signal in CheckShowFirstOpen
                if (this.levelDataController.CurrentLevel < this.adServicesConfig.AOAResumeAdStartLevel && this.gameSessionDataController.OpenTime < this.adServicesConfig.AOAResumeAdStartSession) return;
                if (this.adServicesConfig.IsIntersInsteadAoaResume && this.ShowInterstitialAd(this.thirdPartiesConfig.AdSettings.IntersInsteadAoaResumePlacement, null))
                {
                    this.logService.Log($"onelog: AdServiceWrapper: ShowAOAAdsIfAvailable: ShowInterstitialAd instead of AOA");
                    return;
                }
                #if BRAVESTARS
                if(!this.adServicesConfig.UseAoaResume) return;
                #endif
            }

            var typeToAvailable = string.Join(" | ", this.aoaAdServices.Select(aoa => aoa.GetType().Name + " isReady: " + aoa.IsAOAReady()));
            this.logService.Log($"onelog: AdServiceWrapper: ShowAOAAdsIfAvailable: useAdmob: {this.adServicesConfig.UseAoaAdmob} | {typeToAvailable}");
            foreach (var aoa in this.aoaAdServices.Where(aoaService => aoaService.IsAOAReady()))
            {
                #if ADMOB
                if (this.adServicesConfig.UseAoaAdmob != aoa is AdMobWrapper) continue;
                #endif

                this.signalBus.Fire(new AppOpenCalledSignal(placement));
                aoa.ShowAOAAds(placement);
                this.IsCheckedShowFirstOpen = true;
                this.IsOpenedAOAFirstOpen   = true;
                return;
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

            if (this.IsResumedFromAnotherServices) return;

            this.IsResumedFromAnotherServices = true;
            this.ShowAOAAdsIfAvailable(false).Forget();
        }

        private void OnBannerAdPresented(BannerAdPresentedSignal obj)
        {
            if (this.IsRemovedAds)
                this.bannerAdService.DestroyBannerAd();
            else
            {
                if (!this.IsShowBannerAd || !this.IsMediationBanner) this.InternalHideMediationBannerAd();
                if (!this.IsShowBannerAd || this.IsMediationBanner) this.InternalHideCollapsibleBannerAd();
            }
        }

        private void OnInterstitialDisplayedFailedHandler(InterstitialAdDisplayedFailedSignal obj)
        {
            this.DoOnInterstitialFinishedAction(false);
        }

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

        public virtual bool IsInterstitialAdReady(string place)
        {
            return this.adServices.Any(adService => adService.IsInterstitialAdReady(place));
        }

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
            this.logService.Log($"onelog: ShowInterstitialAd1 {place} force {force} EnableInterstitialAd {isInterstitialAdEnable} CurrentLevel {this.levelDataController.CurrentLevel} InterstitialAdStartLevel {this.adServicesConfig.InterstitialAdStartLevel}");
            if (this.IsRemovedAds || !isInterstitialAdEnable || this.levelDataController.CurrentLevel < this.adServicesConfig.InterstitialAdStartLevel) return false;

            var interstitialAdInterval                                                                                                                                              = this.adServicesConfig.InterstitialAdInterval;
            if (this.thirdPartiesConfig.AdSettings.CustomInterstitialCappingTime.TryGetValue(place, out var cappingTime) && cappingTime.GetCappingTime > -1) interstitialAdInterval = cappingTime.GetCappingTime;

            this.logService.Log($"onelog: ShowInterstitialAd2 {place} force {force} check1 {this.totalNoAdsPlayingTime < interstitialAdInterval} check2 {this.totalNoAdsPlayingTime < this.FirstInterstitialAdsDelayTime}");
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
                this.logService.Warning("onelog: ShowInterstitialAd - InterstitialAd was not loaded");
                onShowInterstitialFinished?.Invoke(false);
                return false;
            }

            this.logService.Log($"onelog: ShowInterstitialAd {place} - {adService.GetType().Name}");

            if (this.thirdPartiesConfig.AdSettings.EnableBreakAds)
                ShowDelayInter(InternalShowInterstitial).Forget();
            else
                InternalShowInterstitial();

            return true;

            async UniTaskVoid ShowDelayInter(Action action)
            {
                #if BRAVESTARS
                this.InternalHideCollapsibleBannerAd();
                #endif
                await this.screenManager.OpenScreen<BreakAdsPopupPresenter>();
                await UniTask.Delay(TimeSpan.FromSeconds(this.thirdPartiesConfig.AdSettings.TimeDelayShowInterBreakAds), DelayType.UnscaledDeltaTime);
                action.Invoke();
            }

            void InternalShowInterstitial()
            {
                this.totalNoAdsPlayingTime = 0;
                var adInfo = new AdInfo(adService.AdPlatform, place, AdFormatConstants.Interstitial);
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

            var adInfo = new AdInfo(adService.AdPlatform, place, AdFormatConstants.Rewarded);
            this.signalBus.Fire(new RewardedAdCalledSignal(place, adInfo));
            this.uiTemplateAdsController.UpdateWatchedRewardedAds();
            this.IsResumedFromAnotherServices = true;
            adService.ShowRewardedAd(place, OnRewardedAdCompleted, onFail);
            return;

            void OnRewardedAdCompleted()
            {
                onComplete?.Invoke();

                if (this.adServicesConfig.ResetInterAdIntervalAfterRewardAd) this.totalNoAdsPlayingTime = 0;
            }
        }

        public virtual bool IsRewardedAdReady(string place)
        {
            return this.adServices.Any(adService => adService.IsRewardedAdReady(place));
        }

        public virtual void RewardedAdOffer(string place)
        {
            this.signalBus.Fire(new RewardedAdOfferSignal(place));
        }

        #endregion

        private string                  mrecPlacement;
        private AdScreenPosition        mrecPosition;
        private AdScreenPosition        mrecOffset;
        private CancellationTokenSource RefreshMRECCts;

        public virtual void ShowMREC(string placement, AdScreenPosition position, AdScreenPosition offset = default)
        {
            if (this.IsRemovedAds || !this.adServicesConfig.EnableMRECAd) return;

            var mrecAdService = this.mrecAdServices.FirstOrDefault(service => service.IsMRECReady(placement, position, offset));
            if (mrecAdService != null)
            {
                mrecAdService.ShowMREC(placement, position, offset);
                this.IsShowMRECAd = true;
                this.logService.Log($"onelog: ShowMREC, placement: {placement}, position: x-{position.x}, y-{position.y}");
                this.mrecPlacement = placement;
                this.mrecPosition  = position;
                this.mrecOffset    = offset;
                this.ScheduleRefreshMREC();
            }
            else
            {
                this.logService.Log("onelog: ShowMREC, MREC no available!");
            }
        }

        public virtual void HideMREC(string placement, AdScreenPosition position)
        {
            this.RefreshMRECCts?.Cancel();
            this.RefreshMRECCts?.Dispose();
            this.RefreshMRECCts = null;

            var mrecAdServices = this.mrecAdServices.Where(service => service.IsMRECReady(placement, position, default)).ToList();
            if (mrecAdServices.Count > 0)
            {
                this.IsShowMRECAd = false;
                foreach (var mrecAdService in mrecAdServices) mrecAdService.HideMREC(placement);
                this.logService.Log($"onelog: HideMREC, placement: {placement}");
            }
        }

        private void DestroyMREC(string placement, AdScreenPosition position)
        {
            var mrecAdServices = this.mrecAdServices.Where(service => service.IsMRECReady(placement, position, default)).ToList();
            if (mrecAdServices.Count > 0)
            {
                this.IsShowMRECAd = false;
                foreach (var mrecAdService in mrecAdServices) mrecAdService.DestroyMREC(placement);
                this.logService.Log($"onelog: DestroyMREC, placement: {placement}");
            }
        }

        private void ScheduleRefreshMREC()
        {
            if (!this.adServicesConfig.EnableMrecRefreshInterval) return;
            UniTask.WaitForSeconds(this.adServicesConfig.MrecRefreshInterval, true, cancellationToken: (this.RefreshMRECCts = new()).Token)
                .ContinueWith(
                    () =>
                    {
                        this.logService.Log($"onelog: ScheduleRefreshMREC");
                        this.DestroyMREC(this.mrecPlacement, this.mrecPosition);
                        this.ShowMREC(this.mrecPlacement, this.mrecPosition, this.mrecOffset);
                    }).Forget();
        }

        #region CollapsibleMREC

        #if THEONE_COLLAPSIBLE_MREC
        private CancellationTokenSource refreshMrecCts;
        private CancellationTokenSource displayMrecCts;

        public virtual void ShowCollapsibleMREC(string placement)
        {
            if (this.IsRemovedAds || !this.adServicesConfig.EnableCollapsibleMrec) return;
            this.ShowMREC(placement, AdScreenPosition.BottomCenter);
            if(!this.IsShowMRECAd) return;
            this.HideBannerAd();
            Debug.Log($"oneLog: SHOW collapsible mrec, placement: {placement}");
            this.signalBus.Fire(new UITemplateOnUpdateCollapMrecStateSignal(true, placement));
            UniTask.WaitForSeconds(this.adServicesConfig.CollapsibleMrecDisplayTime, true, cancellationToken: (this.displayMrecCts = new()).Token)
                .ContinueWith(
                    () =>
                    {
                        this.InternalHideCollapsibleMREC(placement);
                        this.InternalRefreshMrec(placement);
                    }).Forget();
        }

        public virtual void HideCollapsibleMREC(string placement)
        {
            if (this.IsRemovedAds || !this.adServicesConfig.EnableCollapsibleMrec) return;
            this.ResetMrecDisplayCts();
            this.ResetMrecRefreshCts();
            this.InternalHideCollapsibleMREC(placement);
        }

        private void InternalHideCollapsibleMREC(string placement)
        {
            Debug.Log($"oneLog: HIDE collapsible mrec, placement: {placement}");
            this.HideMREC(placement, AdScreenPosition.BottomCenter);
            if (!this.IsShowBannerAd)
            {
                this.ShowBannerAd();
            }
            this.signalBus.Fire(new UITemplateOnUpdateCollapMrecStateSignal(false, placement));
        }

        private void InternalRefreshMrec(string placement)
        {
            UniTask.WaitForSeconds(this.adServicesConfig.CollapsibleMrecInterval, true, cancellationToken: (this.refreshMrecCts = new()).Token)
                .ContinueWith(() =>
                {
                    this.ShowCollapsibleMREC(placement);
                }).Forget();
        }

        public void InternalCloseCollapsibleMREC(string placement)
        {
            this.ResetMrecDisplayCts();
            this.InternalHideCollapsibleMREC(placement);
            this.InternalRefreshMrec(placement);
        }

        private void ResetMrecRefreshCts()
        {
            this.refreshMrecCts?.Cancel();
            this.refreshMrecCts?.Dispose();
            this.refreshMrecCts = null;
        }

        private void ResetMrecDisplayCts()
        {
            this.displayMrecCts?.Cancel();
            this.displayMrecCts?.Dispose();
            this.displayMrecCts = null;
        }

        #endif

        #endregion

        #region NativeOverlayAd

        #if ADMOB

        private float lastTimeShowNativeOverAd = Time.realtimeSinceStartup;

        public virtual async void ShowNativeOverlayInterAd(string placement, Action<bool> onComplete)
        {
            var canShowNativeOverlayAd = Time.realtimeSinceStartup - this.lastTimeShowNativeOverAd > this.adServicesConfig.NativeOverlayInterCappingTime;
            if (this.adServicesConfig.NativeOverlayInterEnable && canShowNativeOverlayAd)
            {
                await this.screenManager.OpenScreen<NativeOverlayInterPopupPresenter, NativeOverlayInterModel>(new (placement, onComplete));
                this.lastTimeShowNativeOverAd = Time.realtimeSinceStartup;
            }
            else
            {
                this.ShowInterstitialAd(placement, onComplete);
            }
        }

        public virtual void ShowNativeOverlayAd(AdViewPosition adViewPosition)
        {
            if (this.IsRemovedAds) return;
            this.nativeOverlayWrapper.ShowAd(adViewPosition);
        }

        public virtual void HideNativeOverlayAd()
        {
            if (this.IsRemovedAds) return;
            this.nativeOverlayWrapper.HideAd();
        }

        #endif

        #endregion

        private void OnRemoveAdsComplete()
        {
            this.signalBus.Fire(new UITemplateOnUpdateCollapMrecStateSignal(false, ""));
            foreach (var mrecAdService in this.mrecAdServices)
            {
                mrecAdService.HideAllMREC();
            }
            this.collapsibleBannerAd.DestroyCollapsibleBannerAd();
            this.bannerAdService.DestroyBannerAd();
        }

        public bool IsRemovedAds => this.adServices.Any(adService => adService.IsRemoveAds());

        public void Tick()
        {
            if (Time.unscaledDeltaTime < 1) this.totalNoAdsPlayingTime += Time.unscaledDeltaTime;

            if (!this.IsCheckedShowFirstOpen && this.gameSessionDataController.OpenTime >= this.adServicesConfig.AOAStartSession) this.CheckShowFirstOpen();
        }

        public void RemoveAds()
        {
            foreach (var adService in this.adServices) adService.RemoveAds();
            foreach (var adService in this.nativeAdsServices) adService.RemoveAds();
            this.nativeOverlayWrapper.DestroyAd();
            this.OnRemoveAdsComplete();
            this.signalBus.Fire<OnRemoveAdsSucceedSignal>();
        }
    }
}