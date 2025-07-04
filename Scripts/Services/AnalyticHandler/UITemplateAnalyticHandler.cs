namespace TheOneStudio.UITemplate.UITemplate.Services
{
    using System;
    using System.Collections.Generic;
    using Core.AdsServices.Signals;
    using Core.AnalyticServices;
    using Core.AnalyticServices.CommonEvents;
    using Core.AnalyticServices.Data;
    using Core.AnalyticServices.Signal;
    using GameFoundation.DI;
    using GameFoundation.Scripts.UIModule.ScreenFlow.Managers;
    using GameFoundation.Scripts.UIModule.ScreenFlow.Signals;
    using GameFoundation.Scripts.Utilities.ApplicationServices;
    using GameFoundation.Signals;
    using ServiceImplementation;
    using ServiceImplementation.IAPServices.Signals;
    using TheOneStudio.UITemplate.UITemplate.Models.Controllers;
    using TheOneStudio.UITemplate.UITemplate.Scripts.Signals;
    using TheOneStudio.UITemplate.UITemplate.Signals;
    using TheOneStudio.UITemplate.UITemplate.ThirdPartyServices.AnalyticEvents;
    using TheOneStudio.UITemplate.UITemplate.ThirdPartyServices.AnalyticEvents.CommonEvents;
    using UnityEngine.Scripting;
#if WIDO
    using System.Collections.Generic;
#endif
#if THEONE_FTUE
    using TheOne.FTUE;
#endif

    public class UITemplateAnalyticHandler : IInitializable, IDisposable
    {
        #region inject

        protected readonly SignalBus                           signalBus;
        protected readonly IAnalyticServices                   analyticServices;
        protected readonly IAnalyticEventFactory               analyticEventFactory;
        protected readonly UITemplateLevelDataController       uiTemplateLevelDataController;
        protected readonly UITemplateInventoryDataController   uITemplateInventoryDataController;
        protected readonly UITemplateDailyRewardController     uiTemplateDailyRewardController;
        protected readonly UITemplateGameSessionDataController uITemplateGameSessionDataController;
        protected readonly IScreenManager                      screenManager;
        private readonly   UITemplateAdsController             uiTemplateAdsController;

        #endregion

        [Preserve]
        public UITemplateAnalyticHandler
        (
            SignalBus signalBus,
            IAnalyticServices analyticServices,
            IAnalyticEventFactory analyticEventFactory,
            UITemplateLevelDataController uiTemplateLevelDataController,
            UITemplateInventoryDataController uITemplateInventoryDataController,
            UITemplateDailyRewardController uiTemplateDailyRewardController,
            UITemplateGameSessionDataController uITemplateGameSessionDataController,
            IScreenManager screenManager,
            UITemplateAdsController uiTemplateAdsController
        )
        {
            this.signalBus                           = signalBus;
            this.analyticServices                    = analyticServices;
            this.analyticEventFactory                = analyticEventFactory;
            this.uiTemplateLevelDataController       = uiTemplateLevelDataController;
            this.uITemplateInventoryDataController   = uITemplateInventoryDataController;
            this.uiTemplateDailyRewardController     = uiTemplateDailyRewardController;
            this.uITemplateGameSessionDataController = uITemplateGameSessionDataController;
            this.screenManager                       = screenManager;
            this.uiTemplateAdsController             = uiTemplateAdsController;
        }

        public void Track(IEvent trackEvent)
        {
            if (trackEvent is CustomEvent customEvent && string.IsNullOrEmpty(customEvent.EventName)) return;

            this.analyticEventFactory.ForceUpdateAllProperties();

            this.analyticServices.Track(trackEvent);
        }

        public virtual void Initialize()
        {
            this.analyticServices.Start();

            //Game events
            this.signalBus.Subscribe<TutorialCompletionSignal>(this.TutorialCompletionHandler);
            this.signalBus.Subscribe<LevelStartedSignal>(this.LevelStartedHandler);
            this.signalBus.Subscribe<LevelEndedSignal>(this.LevelEndedHandler);
            this.signalBus.Subscribe<LevelSkippedSignal>(this.LevelSkippedHandler);
            this.signalBus.Subscribe<OnUpdateCurrencySignal>(this.UpdateCurrencyHandler);
            this.signalBus.Subscribe<ScreenShowSignal>(this.ScreenShowHandler);

            //Banner ads
            this.signalBus.Subscribe<BannerAdLoadedSignal>(this.BannerShowHandler);
            this.signalBus.Subscribe<BannerAdLoadedSignal>(this.BannerLoadHandler);
            this.signalBus.Subscribe<BannerAdLoadFailedSignal>(this.BannerLoadFailHandler);

            //Interstitial ads
            this.signalBus.Subscribe<InterstitialAdEligibleSignal>(this.InterstitialAdEligibleHandler);
            this.signalBus.Subscribe<InterstitialAdCalledSignal>(this.InterstitialAdCalledHandler);
            this.signalBus.Subscribe<InterstitialAdClickedSignal>(this.InterstitialAdClickedHandler);
            this.signalBus.Subscribe<InterstitialAdLoadedSignal>(this.InterstitialAdLoadedHandler);
            this.signalBus.Subscribe<InterstitialAdLoadFailedSignal>(this.InterstitialDownloadAdFailedHandler);
            this.signalBus.Subscribe<InterstitialAdDisplayedSignal>(this.InterstitialAdDisplayedHandler);
            this.signalBus.Subscribe<InterstitialAdClosedSignal>(this.OnInterstitialAdClosed);

            //Rewarded Interstitial ads
            this.signalBus.Subscribe<RewardedInterstitialAdCompletedSignal>(this.RewardedInterstitialAdDisplayedHandler);

            //Reward ads
            this.signalBus.Subscribe<RewardedAdOfferSignal>(this.RewardedAdOfferHandler);
            this.signalBus.Subscribe<RewardedAdEligibleSignal>(this.RewardedAdEligibleHandler);
            this.signalBus.Subscribe<RewardedAdCalledSignal>(this.RewardedAdCalledHandler);
            this.signalBus.Subscribe<RewardedAdClickedSignal>(this.RewardedAdClickedHandler);
            this.signalBus.Subscribe<RewardedAdDisplayedSignal>(this.RewardedAdDisplayedHandler);
            this.signalBus.Subscribe<RewardedAdLoadFailedSignal>(this.RewardedAdLoadFailedHandler);
            this.signalBus.Subscribe<RewardedAdShowFailedSignal>(this.RewardedAdShowFailedHandler);
            this.signalBus.Subscribe<RewardedAdLoadedSignal>(this.RewardedAdDownloadedHandler);
            this.signalBus.Subscribe<RewardedAdCompletedSignal>(this.OnRewardedAdCompleted);
            this.signalBus.Subscribe<RewardedSkippedSignal>(this.OnRewardedAdSkipped);

            //App open
            this.signalBus.Subscribe<AppOpenFullScreenContentOpenedSignal>(this.AppOpenFullScreenContentOpenedHandler);
            this.signalBus.Subscribe<AppOpenFullScreenContentFailedSignal>(this.AppOpenFullScreenContentFailedHandler);
            this.signalBus.Subscribe<AppOpenFullScreenContentClosedSignal>(this.AppOpenFullScreenContentClosedHandler);
            this.signalBus.Subscribe<AppOpenLoadedSignal>(this.AppOpenLoadedHandler);
            this.signalBus.Subscribe<AppOpenLoadFailedSignal>(this.AppOpenLoadFailedHandler);
            this.signalBus.Subscribe<AppOpenEligibleSignal>(this.AppOpenEligibleHandler);
            this.signalBus.Subscribe<AppOpenCalledSignal>(this.AppOpenCalledHandler);
            this.signalBus.Subscribe<AppOpenClickedSignal>(this.AppOpenClickedHandler);

            //Ad revenue
            this.signalBus.Subscribe<AdRevenueSignal>(this.AddRevenueHandler);

            // IAP
            this.signalBus.Subscribe<OnIAPPurchaseSuccessSignal>(this.OnIAPPurchaseSuccess);
            this.signalBus.Subscribe<OnIAPPurchaseFailedSignal>(this.OnIAPPurchaseFailed);

            this.signalBus.Subscribe<PopupShowedSignal>(this.PopupShowedHandler);

            // Game Life Circle
            this.signalBus.Subscribe<ApplicationQuitSignal>(this.OnApplicationQuitHandler);

            // Attribution changed
            this.signalBus.Subscribe<AttributionChangedSignal>(this.OnAttributionChangedHandler);

#if THEONE_FTUE
            this.signalBus.Subscribe<FTUECompletedSignal>(this.OnFTUECompleted);
#endif

            this.TotalDaysPlayedChange();
        }
        
        private void OnAttributionChangedHandler(AttributionChangedSignal obj)
        {
            var eventParameters = new Dictionary<string, object>(obj.EventProperties);
            eventParameters.Add("session", this.uITemplateGameSessionDataController.OpenTime);
            eventParameters.Add("session_timestamp", this.uITemplateGameSessionDataController.SessionTimeStamp);
            this.signalBus.Fire(new EventTrackedSignal()
            {
                TrackedEvent = new CustomEvent()
                {
                    EventName       = "AttributionChanged",
                    EventProperties = eventParameters
                },
                ChangedProps = obj.EventProperties
            });
        }

#if THEONE_FTUE
        private void OnFTUECompleted(FTUECompletedSignal obj)
        {
            this.Track(new CustomEvent()
            {
                EventName = $"ftue_completed",
                EventProperties = new()
                {
                    { "step_id", obj.FTUEId },
                },
            });
        }
#endif

        private void OnApplicationQuitHandler()
        {
            this.isLevelAbandoned = true;
            this.uiTemplateLevelDataController.LoseCurrentLevel();
        }

        private static readonly List<int> SumAdsEventList = new() { 2, 3, 4, 5, 6, 10 };
        protected virtual void AddRevenueHandler(AdRevenueSignal obj)
        {
            switch (obj.AdsRevenueEvent.AdFormat)
            {
                case AdFormatConstants.Rewarded:
                case AdFormatConstants.Interstitial:
                    this.uiTemplateAdsController.CountAdsImpression(obj.AdsRevenueEvent.Revenue);
                    foreach (var sumAmount in SumAdsEventList)
                    {
                        if (this.uiTemplateAdsController.TryGetCircleSumInterstitialAndRewardedAdsRevenue(sumAmount, out var sum))
                        {
                            this.Track(new CustomEvent()
                            {
                                EventName = $"ad_engagement_value_{sumAmount}",
                                EventProperties = new()
                                {
                                    { "currency", obj.AdsRevenueEvent.Currency },
                                    { "revenue", sum },
                                    { "value", sum },
                                },
                            });

                            this.Track(new CustomEvent()
                            {
                                EventName = $"ad_impression_{sumAmount}",
                                EventProperties = new()
                                {
                                    { "currency", obj.AdsRevenueEvent.Currency },
                                    { "revenue", sum },
                                    { "value", sum },
                                },
                            });
                        }
                    }

                    break;
            }

#if WIDO
            var paramDic = new Dictionary<string, object>()
            {
                { "ad_platform", obj.AdsRevenueEvent.AdNetwork },
                { "placement", obj.AdsRevenueEvent.Placement },
            };

            switch (obj.AdsRevenueEvent.AdFormat)
            {
                case "Banner":
                    this.Track(new CustomEvent()
                    {
                        EventName = "banner_show_success",
                        EventProperties = paramDic,
                    });
                    break;
                case "CollapsibleBanner":
                    this.Track(new CustomEvent()
                    {
                        EventName = "collap_banner_show_success",
                        EventProperties = paramDic,
                    });
                    break;
                case "MREC":
                    this.Track(new CustomEvent()
                    {
                        EventName = "mrec_show_success",
                        EventProperties = paramDic,
                    });
                    break;
            }
#endif
        }

        private void AppOpenClickedHandler(AppOpenClickedSignal obj) { this.Track(this.analyticEventFactory.AppOpenClicked(obj.Placement)); }

        private void AppOpenCalledHandler(AppOpenCalledSignal obj) { this.Track(this.analyticEventFactory.AppOpenCalled(obj.Placement)); }

        private void AppOpenEligibleHandler(AppOpenEligibleSignal obj) { this.Track(this.analyticEventFactory.AppOpenEligible(obj.Placement)); }

        private void AppOpenLoadFailedHandler(AppOpenLoadFailedSignal obj) { this.Track(this.analyticEventFactory.AppOpenLoadFailed()); }

        private void AppOpenLoadedHandler(AppOpenLoadedSignal obj) { this.Track(this.analyticEventFactory.AppOpenLoaded()); }

        private void AppOpenFullScreenContentClosedHandler(AppOpenFullScreenContentClosedSignal obj) { this.Track(this.analyticEventFactory.AppOpenFullScreenContentClosed(obj.Placement)); }

        private void AppOpenFullScreenContentFailedHandler(AppOpenFullScreenContentFailedSignal obj) { this.Track(this.analyticEventFactory.AppOpenFullScreenContentFailed(obj.Placement)); }

        private void AppOpenFullScreenContentOpenedHandler(AppOpenFullScreenContentOpenedSignal obj) { this.Track(this.analyticEventFactory.AppOpenFullScreenContentOpened(obj.Placement)); }

        private void OnRewardedAdSkipped(RewardedSkippedSignal obj) { this.Track(this.analyticEventFactory.RewardedVideoShowCompleted(0, obj.Placement, false)); }

        private void OnRewardedAdCompleted(RewardedAdCompletedSignal obj) { this.Track(this.analyticEventFactory.RewardedVideoShowCompleted(0, obj.Placement, true)); }

        private void OnInterstitialAdClosed(InterstitialAdClosedSignal obj) { this.Track(this.analyticEventFactory.InterstitialShowCompleted(0, obj.Placement)); }

        #region Interstitial Ads Signal Handler

        private void InterstitialAdEligibleHandler(InterstitialAdEligibleSignal obj)
        {
            this.Track(this.analyticEventFactory.InterstitialEligible(obj.Placement));
            this.Track(new CustomEvent { EventName = $"Inters_Eligible_{obj.Placement}" });
        }

        private void InterstitialAdClickedHandler(InterstitialAdClickedSignal obj) { this.Track(this.analyticEventFactory.InterstitialClick(obj.Placement)); }

        private void InterstitialAdLoadedHandler(InterstitialAdLoadedSignal obj) { this.Track(this.analyticEventFactory.InterstitialDownloaded(obj.Placement, obj.LoadingMilis)); }

        private void InterstitialDownloadAdFailedHandler(InterstitialAdLoadFailedSignal obj)
        {
            this.Track(this.analyticEventFactory.InterstitialDownloadFailed(obj.Placement, obj.Message, obj.LoadingMilis));
        }

        protected virtual void InterstitialAdDisplayedHandler(InterstitialAdDisplayedSignal obj)
        {
            this.analyticServices.UserProperties[this.analyticEventFactory.LastAdsPlacementProperty]     = obj.Placement;
            this.analyticServices.UserProperties[this.analyticEventFactory.TotalInterstitialAdsProperty] = obj.Placement;
            this.Track(this.analyticEventFactory.InterstitialShow(this.uiTemplateLevelDataController.GetCurrentLevelData.Level, obj.Placement));
            this.Track(new CustomEvent { EventName = $"Inters_Display_{obj.Placement}" });
        }

        private void InterstitialAdCalledHandler(InterstitialAdCalledSignal obj) { this.Track(this.analyticEventFactory.InterstitialCalled(obj.Placement)); }

        private void RewardedInterstitialAdDisplayedHandler(RewardedInterstitialAdCompletedSignal obj)
        {
            this.analyticServices.UserProperties[this.analyticEventFactory.LastAdsPlacementProperty] = obj.Placement;
            this.analyticServices.UserProperties[this.analyticEventFactory.TotalRewardedAdsProperty] = obj.Placement;
            this.Track(this.analyticEventFactory.RewardedInterstitialAdDisplayed(this.uiTemplateLevelDataController.GetCurrentLevelData.Level, obj.Placement));
        }

        #endregion

        #region Banner ads Signal Handler

        private void BannerShowHandler(BannerAdLoadedSignal obj) { this.Track(this.analyticEventFactory.BannerAdShow()); }

        private void BannerLoadFailHandler(BannerAdLoadFailedSignal obj) { this.Track(this.analyticEventFactory.BannerAdLoadFail(obj.Message)); }

        private void BannerLoadHandler(BannerAdLoadedSignal obj) { this.Track(this.analyticEventFactory.BannerAdLoad()); }

        #endregion

        #region Rewarded Ads Signal Handler

        private void RewardedAdEligibleHandler(RewardedAdEligibleSignal obj)
        {
            this.Track(this.analyticEventFactory.RewardedVideoEligible(obj.Placement));
            this.Track(new CustomEvent { EventName = $"Reward_Eligible_{obj.Placement}" });
        }

        private void RewardedAdShowFailedHandler(RewardedAdShowFailedSignal obj) { this.Track(this.analyticEventFactory.RewardedVideoShowFail(obj.Placement, null)); }

        private void RewardedAdLoadFailedHandler(RewardedAdLoadFailedSignal obj) { this.Track(this.analyticEventFactory.RewardedLoadFail(obj.Placement, obj.Message)); }

        private void RewardedAdOfferHandler(RewardedAdOfferSignal obj) { this.Track(this.analyticEventFactory.RewardedVideoOffer(obj.Placement)); }

        private void RewardedAdDownloadedHandler(RewardedAdLoadedSignal obj) { this.Track(this.analyticEventFactory.RewardedVideoDownloaded(obj.Placement, obj.LoadingTime)); }

        private void RewardedAdClickedHandler(RewardedAdClickedSignal obj) { this.Track(this.analyticEventFactory.RewardedVideoClick(obj.Placement)); }

        private void RewardedAdDisplayedHandler(RewardedAdDisplayedSignal obj)
        {
            this.analyticServices.UserProperties[this.analyticEventFactory.LastAdsPlacementProperty] = obj.Placement;
            this.analyticServices.UserProperties[this.analyticEventFactory.TotalRewardedAdsProperty] = obj.Placement;
            this.Track(this.analyticEventFactory.RewardedVideoShow(this.uiTemplateLevelDataController.GetCurrentLevelData.Level, obj.Placement));
            this.Track(new CustomEvent { EventName = $"Reward_Display_{obj.Placement}" });
        }

        private void RewardedAdCalledHandler(RewardedAdCalledSignal obj) { this.Track(this.analyticEventFactory.RewardedVideoCalled(obj.Placement)); }

        #endregion

        private void PopupShowedHandler(PopupShowedSignal obj) { }

        private void LevelSkippedHandler(LevelSkippedSignal obj) { this.Track(this.analyticEventFactory.LevelSkipped(obj.Level, obj.Time)); }

        protected virtual void LevelEndedHandler(LevelEndedSignal obj)
        {
            var levelData = this.uiTemplateLevelDataController.GetLevelData(obj.Level);

            this.analyticServices.UserProperties[this.analyticEventFactory.LevelMaxProperty] = this.uiTemplateLevelDataController.MaxLevel;

            this.Track(obj.IsWin
                ? this.analyticEventFactory.LevelWin(obj.Level, obj.Time, levelData.WinCount)
                : this.analyticEventFactory.LevelLose(obj.Level, obj.Time, levelData.LoseCount)
            );

            if (obj.IsWin && levelData.WinCount == 1)
            {
                this.Track(this.analyticEventFactory.FirstWin(obj.Level, obj.Time));
            }
            
            this.analyticServices.Track(new CustomEvent
            {
                EventName = "ut_level_end",
                EventProperties = new()
                {
                    { "game_mode", obj.Mode },
                    { "ID", this.uniqueId },
                    { "level_id", obj.Level },
                    { "time_spent", obj.Time },
                    { "context", obj.IsWin ? 1 : 0 },
                    { "level_abandoned", this.isLevelAbandoned ? 1 : 0 },
                    { "attempts", levelData.LoseCount + levelData.WinCount }
                },
            });

            switch (obj.Level)
            {
                case 1:
                    this.Track(new CompleteLevel1());

                    break;
                case 5:
                    this.Track(new CompleteLevel5());
                    if (obj.IsWin)
                    {
                        this.Track(new WinLevel5());
                    }

                    break;
                case 10:
                    this.Track(new CompleteLevel10());
                    if (obj.IsWin)
                    {
                        this.Track(new WinLevel10());
                    }

                    break;
            }
        }

        private void TutorialCompletionHandler(TutorialCompletionSignal obj) { this.Track(this.analyticEventFactory.TutorialCompletion(obj.Success, obj.TutorialId)); }

        private string uniqueId;
        private bool   isLevelAbandoned;

        private void LevelStartedHandler(LevelStartedSignal obj)
        {
            var currentLevelData = this.uiTemplateLevelDataController.GetCurrentLevelData;
            this.analyticServices.UserProperties[this.analyticEventFactory.LastLevelProperty] = currentLevelData.Level;
            this.Track(this.analyticEventFactory.LevelStart(obj.Level, this.uITemplateInventoryDataController.GetCurrencyValue()));

            this.uniqueId = Guid.NewGuid().ToString();
            this.analyticServices.Track(new CustomEvent
            {
                EventName = "ut_level_start",
                EventProperties = new()
                {
                    { "game_mode", obj.Mode },
                    { "ID", this.uniqueId },
                    { "level_id", obj.Level },
                    { "time_stamp", obj.TimeStamp },
                    { "attempts", currentLevelData.LoseCount + currentLevelData.WinCount + 1 }
                },
            });

            if (obj.Level <= 50)
            {
                this.Track(new CustomEvent()
                {
                    EventName = $"play_level_{obj.Level}",
                });
            }
        }

        private void UpdateCurrencyHandler(OnUpdateCurrencySignal obj)
        {
            var level = this.uiTemplateLevelDataController.GetCurrentLevelData.Level;
            if (obj.Amount > 0)
            {
                this.Track(this.analyticEventFactory.EarnVirtualCurrency(obj.Id, obj.Amount, obj.Source, level));
            }
            else if (obj.Amount < 0)
            {
                this.Track(this.analyticEventFactory.SpendVirtualCurrency(obj.Id, obj.Amount, obj.Source, level));
            }

            if (obj.Amount > 0)
                this.analyticServices.UserProperties[this.analyticEventFactory.TotalVirtualCurrencyEarnedProperty] = this.uITemplateInventoryDataController.GetCurrencyData(obj.Id).TotalEarned;
            else
                this.analyticServices.UserProperties[this.analyticEventFactory.TotalVirtualCurrencySpentProperty] =
                    this.uITemplateInventoryDataController.GetCurrencyData(obj.Id).TotalEarned - this.uITemplateInventoryDataController.GetCurrencyData(obj.Id).Value;
        }

        private void ScreenShowHandler(ScreenShowSignal obj)
        {
            this.Track(new CustomEvent()
            {
                EventName = $"Enter{obj.ScreenPresenter.GetType().Name.Replace("Screen", "").Replace("Popup", "").Replace("Presenter", "")}",
            });
        }

        private void TotalDaysPlayedChange()
        {
            this.analyticServices.UserProperties[this.analyticEventFactory.RetentionDayProperty] =
                (int)(DateTime.Now.Date - this.uiTemplateDailyRewardController.GetFirstTimeOpenedDate.Date).TotalDays;
            this.analyticServices.UserProperties[this.analyticEventFactory.DaysPlayedProperty] = this.uITemplateGameSessionDataController.OpenTime;
        }

        private void OnIAPPurchaseSuccess(OnIAPPurchaseSuccessSignal signal)
        {
            this.analyticServices.Track(new IapTransactionDidSucceed
            {
                OfferSku     = signal.Product.Id,
                CurrencyCode = signal.Product.CurrencyCode,
                Price        = decimal.ToDouble(signal.Product.Price),
            });
        }

        private void OnIAPPurchaseFailed(OnIAPPurchaseFailedSignal signal)
        {
            this.analyticServices.Track(new IapTransactionDidFail
            {
                OfferSku     = signal.ProductId,
                ErrorMessage = signal.Error,
            });
        }

        public void Dispose()
        {
            this.signalBus.Unsubscribe<LevelStartedSignal>(this.LevelStartedHandler);
            this.signalBus.Unsubscribe<LevelEndedSignal>(this.LevelEndedHandler);
            this.signalBus.Unsubscribe<LevelSkippedSignal>(this.LevelSkippedHandler);
            this.signalBus.Unsubscribe<OnUpdateCurrencySignal>(this.UpdateCurrencyHandler);
            this.signalBus.Unsubscribe<ScreenShowSignal>(this.ScreenShowHandler);

            this.signalBus.Unsubscribe<BannerAdLoadFailedSignal>(this.BannerLoadFailHandler);
            this.signalBus.Unsubscribe<BannerAdLoadedSignal>(this.BannerLoadHandler);
            this.signalBus.Unsubscribe<BannerAdLoadedSignal>(this.BannerShowHandler);

            this.signalBus.Unsubscribe<InterstitialAdEligibleSignal>(this.InterstitialAdEligibleHandler);
            this.signalBus.Unsubscribe<InterstitialAdCalledSignal>(this.InterstitialAdCalledHandler);
            this.signalBus.Unsubscribe<InterstitialAdClickedSignal>(this.InterstitialAdClickedHandler);
            this.signalBus.Unsubscribe<InterstitialAdLoadedSignal>(this.InterstitialAdLoadedHandler);
            this.signalBus.Unsubscribe<InterstitialAdLoadFailedSignal>(this.InterstitialDownloadAdFailedHandler);
            this.signalBus.Unsubscribe<InterstitialAdDisplayedSignal>(this.InterstitialAdDisplayedHandler);

            this.signalBus.Unsubscribe<RewardedInterstitialAdCompletedSignal>(this.RewardedInterstitialAdDisplayedHandler);
            this.signalBus.Unsubscribe<RewardedAdOfferSignal>(this.RewardedAdOfferHandler);
            this.signalBus.Unsubscribe<RewardedAdEligibleSignal>(this.RewardedAdEligibleHandler);
            this.signalBus.Unsubscribe<RewardedAdCalledSignal>(this.RewardedAdCalledHandler);
            this.signalBus.Unsubscribe<RewardedAdClickedSignal>(this.RewardedAdClickedHandler);
            this.signalBus.Unsubscribe<RewardedAdDisplayedSignal>(this.RewardedAdDisplayedHandler);
            this.signalBus.Unsubscribe<RewardedAdLoadFailedSignal>(this.RewardedAdLoadFailedHandler);
            this.signalBus.Unsubscribe<RewardedAdShowFailedSignal>(this.RewardedAdShowFailedHandler);
            this.signalBus.Unsubscribe<RewardedAdLoadedSignal>(this.RewardedAdDownloadedHandler);

            this.signalBus.Unsubscribe<PopupShowedSignal>(this.PopupShowedHandler);

            //App open
            this.signalBus.Unsubscribe<AppOpenFullScreenContentOpenedSignal>(this.AppOpenFullScreenContentOpenedHandler);
            this.signalBus.Unsubscribe<AppOpenFullScreenContentFailedSignal>(this.AppOpenFullScreenContentFailedHandler);
            this.signalBus.Unsubscribe<AppOpenFullScreenContentClosedSignal>(this.AppOpenFullScreenContentClosedHandler);
            this.signalBus.Unsubscribe<AppOpenLoadedSignal>(this.AppOpenLoadedHandler);
            this.signalBus.Unsubscribe<AppOpenLoadFailedSignal>(this.AppOpenLoadFailedHandler);
            this.signalBus.Unsubscribe<AppOpenEligibleSignal>(this.AppOpenEligibleHandler);
            this.signalBus.Unsubscribe<AppOpenCalledSignal>(this.AppOpenCalledHandler);
        }
    }
}