namespace TheOneStudio.UITemplate.UITemplate.Services
{
    using System;
    using System.Collections.Generic;
    using Core.AdsServices.Signals;
    using Core.AnalyticServices;
    using Core.AnalyticServices.CommonEvents;
    using Core.AnalyticServices.Data;
    using Core.AnalyticServices.Signal;
    using GameFoundation.Scripts.UIModule.ScreenFlow.Signals;
    using TheOneStudio.UITemplate.UITemplate.Models.Controllers;
    using TheOneStudio.UITemplate.UITemplate.Scripts.Signals;
    using TheOneStudio.UITemplate.UITemplate.Signals;
    using TheOneStudio.UITemplate.UITemplate.ThirdPartyServices.AnalyticEvents;
    using Zenject;

    public class UITemplateAnalyticHandler : IInitializable, IDisposable
    {
        #region inject

        private readonly SignalBus                         signalBus;
        private readonly IAnalyticServices                 analyticServices;
        private readonly List<IAnalyticEventFactory>       analyticEventFactories;
        private readonly UITemplateLevelDataController     uiTemplateLevelDataController;
        private readonly UITemplateInventoryDataController uITemplateInventoryDataController;
        private readonly UITemplateDailyRewardController   uiTemplateDailyRewardController;

        #endregion

        public UITemplateAnalyticHandler(SignalBus signalBus, IAnalyticServices analyticServices, List<IAnalyticEventFactory> analyticEventFactories,
            UITemplateLevelDataController uiTemplateLevelDataController, UITemplateInventoryDataController uITemplateInventoryDataController,
            UITemplateDailyRewardController uiTemplateDailyRewardController)
        {
            this.signalBus                         = signalBus;
            this.analyticServices                  = analyticServices;
            this.analyticEventFactories            = analyticEventFactories;
            this.uiTemplateLevelDataController     = uiTemplateLevelDataController;
            this.uITemplateInventoryDataController = uITemplateInventoryDataController;
            this.uiTemplateDailyRewardController   = uiTemplateDailyRewardController;

            switch (analyticEventFactories.Count)
            {
                case > 1:
                    throw new Exception("Error: More than one analytic event factory found. Please remove one of them (Project Setting/Script Define Symbols).");
                case 0:
                    throw new Exception("Error: No analytic event factory found. Please add one of them (WIDO,ROCKET,ADONE,ABI...) into (Project Setting/Script Define Symbols).");
            }
        }

        public void Track(IEvent trackEvent)
        {
            this.analyticEventFactories.ForEach(x => x.ForceUpdateAllProperties());

            if (trackEvent is CustomEvent customEvent && string.IsNullOrEmpty(customEvent.EventName))
                return;

            this.analyticServices.Track(trackEvent);
        }

        public void DoAnalyticWithFactories(Action<IAnalyticEventFactory> action)
        {
            foreach (var factory in this.analyticEventFactories)
            {
                action(factory);
            }
        }

        public void Initialize()
        {
            this.analyticServices.Start();

            //Game events
            this.signalBus.Subscribe<TutorialCompletionSignal>(this.TutorialCompletionHandler);
            this.signalBus.Subscribe<LevelStartedSignal>(this.LevelStartedHandler);
            this.signalBus.Subscribe<LevelEndedSignal>(this.LevelEndedHandler);
            this.signalBus.Subscribe<LevelSkippedSignal>(this.LevelSkippedHandler);
            this.signalBus.Subscribe<OnUpdateCurrencySignal>(this.UpdateCurrencyHandler);
            this.signalBus.Subscribe<ScreenShowSignal>(this.ScreenShowHandler);

            //Interstitial ads
            this.signalBus.Subscribe<InterstitialAdEligibleSignal>(this.InterstitialAdEligibleHandler);
            this.signalBus.Subscribe<InterstitialAdCalledSignal>(this.InterstitialAdCalledHandler);
            this.signalBus.Subscribe<InterstitialAdClickedSignal>(this.InterstitialAdClickedHandler);
            this.signalBus.Subscribe<InterstitialAdDownloadedSignal>(this.InterstitialAdLoadedHandler);
            this.signalBus.Subscribe<InterstitialAdLoadFailedSignal>(this.InterstitialDownloadAdFailedHandler);
            this.signalBus.Subscribe<InterstitialAdDisplayedSignal>(this.InterstitialAdDisplayedHandler);
            this.signalBus.Subscribe<InterstitialAdClosedSignal>(this.OnInterstitialAdClosed);

            this.signalBus.Subscribe<RewardedInterstitialAdCompletedSignal>(this.RewardedInterstitialAdDisplayedHandler);

            //Reward ads
            this.signalBus.Subscribe<RewardedAdOfferSignal>(this.RewardedAdOfferHandler);
            this.signalBus.Subscribe<RewardedAdEligibleSignal>(this.RewardedAdEligibleHandler);
            this.signalBus.Subscribe<RewardedAdCalledSignal>(this.RewardedAdCalledHandler);
            this.signalBus.Subscribe<RewardedAdLoadClickedSignal>(this.RewardedAdClickedHandler);
            this.signalBus.Subscribe<RewardedAdDisplayedSignal>(this.RewardedAdDisplayedHandler);
            this.signalBus.Subscribe<RewardedAdLoadFailedSignal>(this.RewardedAdFailedHandler);
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


            this.signalBus.Subscribe<PopupShowedSignal>(this.PopupShowedHandler);

            this.TotalDaysPlayedChange();
        }
        
        private void AddRevenueHandler(AdRevenueSignal obj)
        {
            #if WIDO
            var paramDic = new Dictionary<string, object>()
            {
                { "ad_platform", obj.AdsRevenueEvent.AdNetwork },
                { "placement", obj.AdsRevenueEvent.Placement },
            };
            
            switch (obj.AdsRevenueEvent.AdFormat)
            {
                case "Banner":
                    this.DoAnalyticWithFactories(_ => this.Track(new CustomEvent()
                    {
                        EventName       = "banner_show_success",
                        EventProperties = paramDic,
                    }));
                    break;
                case "CollapsibleBanner":
                    this.DoAnalyticWithFactories(_ => this.Track(new CustomEvent()
                    {
                        EventName       = "banner_show_success",
                        EventProperties = paramDic,
                    }));
                    break;
                case "MREC":
                    this.DoAnalyticWithFactories(_ => this.Track(new CustomEvent()
                    {
                        EventName = "mrec_show_success",
                        EventProperties = paramDic,
                    }));
                    break;
            }
            #endif
        }
        
        private void AppOpenClickedHandler(AppOpenClickedSignal obj)
        {
            this.DoAnalyticWithFactories(eventFactory => this.Track(eventFactory.AppOpenClicked()));
        }
        private void AppOpenCalledHandler(AppOpenCalledSignal obj)         { this.DoAnalyticWithFactories(eventFactory => this.Track(eventFactory.AppOpenCalled())); }
        private void AppOpenEligibleHandler(AppOpenEligibleSignal obj)     { this.DoAnalyticWithFactories(eventFactory => this.Track(eventFactory.AppOpenEligible())); }
        private void AppOpenLoadFailedHandler(AppOpenLoadFailedSignal obj) { this.DoAnalyticWithFactories(eventFactory => this.Track(eventFactory.AppOpenLoadFailed())); }
        private void AppOpenLoadedHandler(AppOpenLoadedSignal obj)         { this.DoAnalyticWithFactories(eventFactory => this.Track(eventFactory.AppOpenLoaded())); }
        private void AppOpenFullScreenContentClosedHandler(AppOpenFullScreenContentClosedSignal obj)
        {
            this.DoAnalyticWithFactories(eventFactory => this.Track(eventFactory.AppOpenFullScreenContentClosed()));
        }
        private void AppOpenFullScreenContentFailedHandler(AppOpenFullScreenContentFailedSignal obj)
        {
            this.DoAnalyticWithFactories(eventFactory => this.Track(eventFactory.AppOpenFullScreenContentFailed()));
        }
        private void AppOpenFullScreenContentOpenedHandler(AppOpenFullScreenContentOpenedSignal obj)
        {
            this.DoAnalyticWithFactories(eventFactory => this.Track(eventFactory.AppOpenFullScreenContentOpened()));
        }

        private void OnRewardedAdSkipped(RewardedSkippedSignal obj) { this.DoAnalyticWithFactories(eventFactory => this.Track(eventFactory.RewardedVideoShowCompleted(0, obj.Placement, false))); }

        private void OnRewardedAdCompleted(RewardedAdCompletedSignal obj) { this.DoAnalyticWithFactories(eventFactory => this.Track(eventFactory.RewardedVideoShowCompleted(0, obj.Placement, true))); }

        private void OnInterstitialAdClosed(InterstitialAdClosedSignal obj) { this.DoAnalyticWithFactories(eventFactory => this.Track(eventFactory.InterstitialShowCompleted(0, obj.Placement))); }

        #region Interstitial Ads Signal Handler

        private void InterstitialAdEligibleHandler(InterstitialAdEligibleSignal obj)
        {
            this.DoAnalyticWithFactories(eventFactory =>
            {
                this.Track(eventFactory.InterstitialEligible(obj.Placement));
                this.Track(new CustomEvent { EventName = $"Interstitial_Eligible_{obj.Placement}" });
            });
        }

        private void InterstitialAdClickedHandler(InterstitialAdClickedSignal obj) { this.DoAnalyticWithFactories(eventFactory => this.Track(eventFactory.InterstitialClick(obj.Placement))); }

        private void InterstitialAdLoadedHandler(InterstitialAdDownloadedSignal obj) { this.DoAnalyticWithFactories(eventFactory => this.Track(eventFactory.InterstitialDownloaded(obj.Placement))); }

        private void InterstitialDownloadAdFailedHandler(InterstitialAdLoadFailedSignal obj)
        {
            this.DoAnalyticWithFactories(eventFactory => this.Track(eventFactory.InterstitialDownloadFailed(obj.Placement)));
        }

        private void InterstitialAdDisplayedHandler(InterstitialAdDisplayedSignal obj)
        {
            this.DoAnalyticWithFactories(eventFactory =>
            {
                this.analyticServices.UserProperties[eventFactory.LastAdsPlacementProperty]     = obj.Placement;
                this.analyticServices.UserProperties[eventFactory.TotalInterstitialAdsProperty] = obj.Placement;
                this.Track(eventFactory.InterstitialShow(this.uiTemplateLevelDataController.GetCurrentLevelData.Level, obj.Placement));
                this.Track(new CustomEvent { EventName = $"Interstitial_Displayed_{obj.Placement}" });
            });
        }

        private void InterstitialAdCalledHandler(InterstitialAdCalledSignal obj) { this.DoAnalyticWithFactories(eventFactory => this.Track(eventFactory.InterstitialCalled(obj.Placement))); }

        private void RewardedInterstitialAdDisplayedHandler(RewardedInterstitialAdCompletedSignal obj)
        {
            this.DoAnalyticWithFactories(eventFactory =>
            {
                this.analyticServices.UserProperties[eventFactory.LastAdsPlacementProperty] = obj.Placement;
                this.analyticServices.UserProperties[eventFactory.TotalRewardedAdsProperty] = obj.Placement;
                this.Track(eventFactory.RewardedInterstitialAdDisplayed(this.uiTemplateLevelDataController.GetCurrentLevelData.Level, obj.Placement));
            });
        }

        #endregion

        #region Rewarded Ads Signal Handler

        private void RewardedAdEligibleHandler(RewardedAdEligibleSignal obj)
        {
            this.DoAnalyticWithFactories(eventFactory =>
            {
                this.Track(eventFactory.RewardedVideoEligible(obj.Placement));
                this.Track(new CustomEvent { EventName = $"Rewarded_Eligible_{obj.Placement}" });
            });
        }

        private void RewardedAdFailedHandler(RewardedAdLoadFailedSignal obj)
        {
            this.DoAnalyticWithFactories(eventFactory => this.Track(eventFactory.RewardedVideoShowFail(obj.Placement, obj.Message)));
        }

        private void RewardedAdOfferHandler(RewardedAdOfferSignal obj) { this.DoAnalyticWithFactories(eventFactory => this.Track(eventFactory.RewardedVideoOffer(obj.Placement))); }

        private void RewardedAdDownloadedHandler(RewardedAdLoadedSignal obj) { this.DoAnalyticWithFactories(eventFactory => this.Track(eventFactory.RewardedVideoDownloaded(obj.Placement))); }

        private void RewardedAdClickedHandler(RewardedAdLoadClickedSignal obj) { this.DoAnalyticWithFactories(eventFactory => this.Track(eventFactory.RewardedVideoClick(obj.Placement))); }

        private void RewardedAdDisplayedHandler(RewardedAdDisplayedSignal obj)
        {
            this.DoAnalyticWithFactories(eventFactory =>
            {
                this.analyticServices.UserProperties[eventFactory.LastAdsPlacementProperty] = obj.Placement;
                this.analyticServices.UserProperties[eventFactory.TotalRewardedAdsProperty] = obj.Placement;
                this.Track(eventFactory.RewardedVideoShow(this.uiTemplateLevelDataController.GetCurrentLevelData.Level, obj.Placement));
                this.Track(new CustomEvent { EventName = $"Rewarded_Displayed_{obj.Placement}" });
            });
        }

        private void RewardedAdCalledHandler(RewardedAdCalledSignal obj) { this.DoAnalyticWithFactories(eventFactory => this.Track(eventFactory.RewardedVideoCalled(obj.Placement))); }

        #endregion

        private void PopupShowedHandler(PopupShowedSignal obj) { }

        private void LevelSkippedHandler(LevelSkippedSignal obj) { this.DoAnalyticWithFactories(eventFactory => this.Track(eventFactory.LevelSkipped(obj.Level, obj.Time))); }

        private void LevelEndedHandler(LevelEndedSignal obj)
        {
            var levelData = this.uiTemplateLevelDataController.GetLevelData(obj.Level);

            this.DoAnalyticWithFactories(eventFactory =>
            {
                this.analyticServices.UserProperties[eventFactory.LevelMaxProperty] = this.uiTemplateLevelDataController.MaxLevel;

                this.Track(obj.IsWin
                    ? eventFactory.LevelWin(obj.Level, obj.Time, levelData.WinCount)
                    : eventFactory.LevelLose(obj.Level, obj.Time, levelData.LoseCount));

                if (obj.IsWin && levelData.WinCount == 1)
                {
                    this.Track(eventFactory.FirstWin(obj.Level, obj.Time));
                }
            });
        }

        private void TutorialCompletionHandler(TutorialCompletionSignal obj) { this.DoAnalyticWithFactories(eventFactory => this.Track(eventFactory.TutorialCompletion(obj.Success, obj.TutorialId))); }

        private void LevelStartedHandler(LevelStartedSignal obj)
        {
            this.DoAnalyticWithFactories(eventFactory =>
            {
                this.analyticServices.UserProperties[eventFactory.LastLevelProperty] = this.uiTemplateLevelDataController.GetCurrentLevelData.Level;
                this.Track(eventFactory.LevelStart(obj.Level, this.uITemplateInventoryDataController.GetCurrencyValue()));
                
                if (obj.Level > 50) return;
                this.Track(new CustomEvent()
                {
                    EventName = $"play_level_{obj.Level}",
                });
            });
        }

        private void UpdateCurrencyHandler(OnUpdateCurrencySignal obj)
        {
            this.DoAnalyticWithFactories(eventFactory =>
            {
                if (obj.Amount > 0)
                    this.analyticServices.UserProperties[eventFactory.TotalVirtualCurrencyEarnedProperty] = this.uITemplateInventoryDataController.GetCurrencyData(obj.Id).TotalEarned;
                else
                    this.analyticServices.UserProperties[eventFactory.TotalVirtualCurrencySpentProperty] =
                        this.uITemplateInventoryDataController.GetCurrencyData(obj.Id).TotalEarned - this.uITemplateInventoryDataController.GetCurrencyData(obj.Id).Value;
            });
        }
        
             
        private void ScreenShowHandler(ScreenShowSignal obj)
        {
            this.Track(new CustomEvent()
            {
                EventName = $"Enter{obj.ScreenPresenter.GetType().Name.Replace("Screen", "").Replace("Popup","").Replace("Presenter","")}",
            });
        }

        private void TotalDaysPlayedChange()
        {
            this.DoAnalyticWithFactories(eventFactory =>
            {
                this.analyticServices.UserProperties[eventFactory.DaysPlayedProperty] = (int)(DateTime.Now.Date - this.uiTemplateDailyRewardController.GetFirstTimeOpenedDate.Date).TotalDays;
            });
        }

        public void Dispose()
        {
            this.signalBus.Unsubscribe<LevelStartedSignal>(this.LevelStartedHandler);
            this.signalBus.Unsubscribe<LevelEndedSignal>(this.LevelEndedHandler);
            this.signalBus.Unsubscribe<LevelSkippedSignal>(this.LevelSkippedHandler);
            this.signalBus.Unsubscribe<OnUpdateCurrencySignal>(this.UpdateCurrencyHandler);
            this.signalBus.Unsubscribe<ScreenShowSignal>(this.ScreenShowHandler);
            
            this.signalBus.Unsubscribe<InterstitialAdEligibleSignal>(this.InterstitialAdEligibleHandler);
            this.signalBus.Unsubscribe<InterstitialAdCalledSignal>(this.InterstitialAdCalledHandler);
            this.signalBus.Unsubscribe<InterstitialAdClickedSignal>(this.InterstitialAdClickedHandler);
            this.signalBus.Unsubscribe<InterstitialAdDownloadedSignal>(this.InterstitialAdLoadedHandler);
            this.signalBus.Unsubscribe<InterstitialAdLoadFailedSignal>(this.InterstitialDownloadAdFailedHandler);
            this.signalBus.Unsubscribe<InterstitialAdDisplayedSignal>(this.InterstitialAdDisplayedHandler);
            this.signalBus.Unsubscribe<RewardedInterstitialAdCompletedSignal>(this.RewardedInterstitialAdDisplayedHandler);
            this.signalBus.Unsubscribe<RewardedAdOfferSignal>(this.RewardedAdOfferHandler);
            this.signalBus.Unsubscribe<RewardedAdEligibleSignal>(this.RewardedAdEligibleHandler);
            this.signalBus.Unsubscribe<RewardedAdCalledSignal>(this.RewardedAdCalledHandler);
            this.signalBus.Unsubscribe<RewardedAdLoadClickedSignal>(this.RewardedAdClickedHandler);
            this.signalBus.Unsubscribe<RewardedAdDisplayedSignal>(this.RewardedAdDisplayedHandler);
            this.signalBus.Unsubscribe<RewardedAdLoadFailedSignal>(this.RewardedAdFailedHandler);
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