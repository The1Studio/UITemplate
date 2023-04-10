namespace TheOneStudio.UITemplate.UITemplate.Services
{
    using System;
    using System.Collections.Generic;
    using Core.AdsServices;
    using Core.AdsServices.Signals;
    using Core.AnalyticServices;
    using Core.AnalyticServices.CommonEvents;
    using Core.AnalyticServices.Data;
    using GameFoundation.Scripts.UIModule.ScreenFlow.Signals;
    using GameFoundation.Scripts.Utilities.LogService;
    using TheOneStudio.UITemplate.UITemplate.Models;
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
        private readonly List<IAnalyticEventFactory>       analyticEventList;
        private readonly UITemplateUserLevelData           uiTemplateUserLevelData;
        private readonly IAdServices                       adServices;
        private readonly ILogService                       logService;
        private readonly UITemplateLevelDataController     uiTemplateLevelDataController;
        private readonly UITemplateInventoryDataController uITemplateInventoryDataController;

        #endregion

        public UITemplateAnalyticHandler(SignalBus signalBus, IAnalyticServices analyticServices, List<IAnalyticEventFactory> analyticEventList, UITemplateUserLevelData uiTemplateUserLevelData,
            IAdServices adServices, ILogService logService, UITemplateLevelDataController uiTemplateLevelDataController, UITemplateInventoryDataController uITemplateInventoryDataController)
        {
            this.signalBus                         = signalBus;
            this.analyticServices                  = analyticServices;
            this.analyticEventList                 = analyticEventList;
            this.uiTemplateUserLevelData           = uiTemplateUserLevelData;
            this.adServices                        = adServices;
            this.logService                        = logService;
            this.uiTemplateLevelDataController     = uiTemplateLevelDataController;
            this.uITemplateInventoryDataController = uITemplateInventoryDataController;

            switch (analyticEventList.Count)
            {
                case > 1:
                    throw new Exception("Error: More than one analytic event factory found. Please remove one of them (Project Setting/Script Define Symbols).");
                case 0:
                    throw new Exception("Error: No analytic event factory found. Please add one of them (WIDO,ROCKET,ADONE,ABI...) into (Project Setting/Script Define Symbols).");

                    break;
            }
        }

        private void Track(IEvent trackEvent)
        {
            this.analyticEventList.ForEach(x => x.ForceUpdateAllProperties());

            if (trackEvent is CustomEvent customEvent && string.IsNullOrEmpty(customEvent.EventName))
                return;

            this.analyticServices.Track(trackEvent);
        }

        public void Initialize()
        {
            this.analyticServices.Start();

            //Game events
            this.signalBus.Subscribe<TutorialCompletionSignal>(this.TutorialCompletionHandler);
            
            this.signalBus.Subscribe<LevelStartedSignal>(this.LevelStartedHandler);
            this.signalBus.Subscribe<LevelEndedSignal>(this.LevelEndedHandler);
            this.signalBus.Subscribe<LevelSkippedSignal>(this.LevelSkippedHandler);
            this.signalBus.Subscribe<TotalVirtualCurrencySpentSignal>(this.TotalVirtualCurrencySpentHandler);
            this.signalBus.Subscribe<TotalVirtualCurrencyEarnedSignal>(this.TotalVirtualCurrencyEarnedHandler);
            this.signalBus.Subscribe<DaysPlayedSignal>(this.DaysPlayedHandler);
            
            this.signalBus.Subscribe<InterstitialAdEligibleSignal>(this.InterstitialAdEligibleHandler);
            this.signalBus.Subscribe<InterstitialAdCalledSignal>(this.InterstitialAdCalledHandler);
            this.signalBus.Subscribe<InterstitialAdClickedSignal>(this.InterstitialAdClickedHandler);
            this.signalBus.Subscribe<InterstitialAdDownloadedSignal>(this.InterstitialAdLoadedHandler);
            this.signalBus.Subscribe<InterstitialAdLoadFailedSignal>(this.InterstitialAdFailedHandler);
            this.signalBus.Subscribe<InterstitialAdDisplayedSignal>(this.InterstitialAdDisplayedHandler);
            
            this.signalBus.Subscribe<RewardedInterstitialAdDisplayedSignal>(this.RewardedInterstitialAdDisplayedHandler);
            this.signalBus.Subscribe<RewardedAdOfferSignal>(this.RewardedAdOfferHandler);
            this.signalBus.Subscribe<RewardedAdEligibleSignal>(this.RewardedAdEligibleHandler);
            this.signalBus.Subscribe<RewardedAdCalledSignal>(this.RewardedAdCalledHandler);
            this.signalBus.Subscribe<RewardedAdLoadClickedSignal>(this.RewardedAdClickedHandler);
            this.signalBus.Subscribe<RewardedAdDisplayedSignal>(this.RewardedAdDisplayedHandler);
            this.signalBus.Subscribe<RewardedAdLoadFailedSignal>(this.RewardedAdFailedHandler);
            this.signalBus.Subscribe<RewardedAdLoadedSignal>(this.RewardedAdDownloadedHandler);
            
            this.signalBus.Subscribe<PopupShowedSignal>(this.PopupShowedHandler);

            //Ads events
            this.adServices.RewardedAdCompleted     += this.RewardedAdCompletedHandler;
            this.adServices.RewardedAdSkipped       += this.RewardedAdSkippedHandler;
            this.adServices.InterstitialAdCompleted += this.InterstitialAdCompletedHandler;
        }

        #region Interstitial Ads Signal Handler

        private void InterstitialAdEligibleHandler(InterstitialAdEligibleSignal obj)
        {
            foreach (var analytic in this.analyticEventList)
            {
                this.Track(analytic.InterstitialEligible(obj.place));
            }
        }

        private void InterstitialAdClickedHandler(InterstitialAdClickedSignal obj)
        {
            foreach (var analytic in this.analyticEventList)
            {
                this.Track(analytic.InterstitialClick(obj.Placement));
            }
        }

        private void InterstitialAdLoadedHandler(InterstitialAdDownloadedSignal obj)
        {
            foreach (var analytic in this.analyticEventList)
            {
                this.Track(analytic.InterstitialDownloaded(obj.Placement));
            }
        }

        private void InterstitialAdFailedHandler(InterstitialAdLoadFailedSignal obj)
        {
            foreach (var analytic in this.analyticEventList)
            {
                this.Track(analytic.InterstitialShowFail(obj.Placement, obj.Message));
            }
        }

        private void InterstitialAdCompletedHandler(InterstitialAdNetwork arg1, string arg2)
        {
            foreach (var analytic in this.analyticEventList)
            {
                this.Track(analytic.InterstitialShowCompleted(0, arg2));
            }
        }

        private void InterstitialAdDisplayedHandler(InterstitialAdDisplayedSignal obj)
        {
            foreach (var analytic in this.analyticEventList)
            {
                this.analyticServices.UserProperties[analytic.LastAdsPlacementProperty]     = obj.Placement;
                this.analyticServices.UserProperties[analytic.TotalInterstitialAdsProperty] = obj.Placement;
                this.Track(analytic.InterstitialShow(this.uiTemplateUserLevelData.CurrentLevel, obj.Placement));
            }
        }

        private void InterstitialAdCalledHandler(InterstitialAdCalledSignal obj)
        {
            foreach (var analytic in this.analyticEventList)
            {
                this.Track(analytic.InterstitialCalled(obj.place));
            }
        }

        private void RewardedInterstitialAdDisplayedHandler(RewardedInterstitialAdDisplayedSignal obj)
        {
            foreach (var analytic in this.analyticEventList)
            {
                this.analyticServices.UserProperties[analytic.LastAdsPlacementProperty] = obj.Placement;
                this.analyticServices.UserProperties[analytic.TotalRewardedAdsProperty] = obj.Placement;
                this.Track(analytic.RewardedInterstitialAdDisplayed(this.uiTemplateUserLevelData.CurrentLevel, obj.Placement));
            }
        }

        #endregion

        #region Rewarded Ads Signal Handler

        private void RewardedAdEligibleHandler(RewardedAdEligibleSignal obj)
        {
            foreach (var analytic in this.analyticEventList)
            {
                this.Track(analytic.RewardedVideoEligible(obj.place));
            }
        }

        private void RewardedAdFailedHandler(RewardedAdLoadFailedSignal obj)
        {
            foreach (var analytic in this.analyticEventList)
            {
                this.Track(analytic.RewardedVideoShowFail(obj.Placement, obj.Message));
            }
        }

        private void RewardedAdOfferHandler(RewardedAdOfferSignal obj)
        {
            foreach (var analytic in this.analyticEventList)
            {
                this.Track(analytic.RewardedVideoOffer(obj.Place));
            }
        }

        private void RewardedAdDownloadedHandler(RewardedAdLoadedSignal obj)
        {
            foreach (var analytic in this.analyticEventList)
            {
                this.Track(analytic.RewardedVideoDownloaded(obj.Placement));
            }
        }

        private void RewardedAdClickedHandler(RewardedAdLoadClickedSignal obj)
        {
            foreach (var analytic in this.analyticEventList)
            {
                this.Track(analytic.RewardedVideoClick(obj.Placement));
            }
        }

        private void RewardedAdDisplayedHandler(RewardedAdDisplayedSignal obj)
        {
            foreach (var analytic in this.analyticEventList)
            {
                this.analyticServices.UserProperties[analytic.LastAdsPlacementProperty] = obj.Placement;
                this.analyticServices.UserProperties[analytic.TotalRewardedAdsProperty] = obj.Placement;
                this.Track(analytic.RewardedVideoShow(this.uiTemplateUserLevelData.CurrentLevel, obj.Placement));
            }
        }

        private void RewardedAdSkippedHandler(RewardedAdNetwork arg1, string arg2)
        {
            foreach (var analytic in this.analyticEventList)
            {
                this.Track(analytic.RewardedVideoShowCompleted(0, arg2, false));
            }
        }

        private void RewardedAdCompletedHandler(RewardedAdNetwork arg1, string arg2)
        {
            foreach (var analytic in this.analyticEventList)
            {
                this.Track(analytic.RewardedVideoShowCompleted(0, arg2, true));
            }
        }

        private void RewardedAdCalledHandler(RewardedAdCalledSignal obj)
        {
            foreach (var analytic in this.analyticEventList)
            {
                this.Track(analytic.RewardedVideoCalled(obj.place));
            }
        }

        #endregion

        private void PopupShowedHandler(PopupShowedSignal obj) { }

        private void LevelSkippedHandler(LevelSkippedSignal obj)
        {
            foreach (var analytic in this.analyticEventList)
            {
                this.Track(analytic.LevelSkipped(obj.Level, obj.Time));
            }
        }

        private void LevelEndedHandler(LevelEndedSignal obj)
        {
            var levelData = this.uiTemplateLevelDataController.GetLevelData(obj.Level);

            foreach (var analytic in this.analyticEventList)
            {
                this.analyticServices.UserProperties[analytic.LevelMaxProperty] = this.uiTemplateLevelDataController.MaxLevel;

                this.Track(obj.IsWin
                    ? analytic.LevelWin(obj.Level, obj.Time, levelData.WinCount)
                    : analytic.LevelLose(obj.Level, obj.Time, levelData.LoseCount));

                if (obj.IsWin && levelData.WinCount == 1)
                {
                    analytic.FirstWin(obj.Level, obj.Time);
                }

                if (!obj.IsWin)
                {
                    analytic.LevelLose(obj.Level, obj.Time, levelData.LoseCount);
                }
            }
        }

        private void TutorialCompletionHandler(TutorialCompletionSignal obj)
        {
            foreach (var analytic in this.analyticEventList)
            {
                this.Track(analytic.TutorialCompletion(obj.Success, obj.TutorialId));
            }
        }

        private void LevelStartedHandler(LevelStartedSignal obj)
        {
            foreach (var analytic in this.analyticEventList)
            {
                this.analyticServices.UserProperties[analytic.LastLevelProperty] = this.uiTemplateUserLevelData.CurrentLevel;
                this.Track(analytic.LevelStart(obj.Level, this.uITemplateInventoryDataController.GetCurrencyValue()));
            }
        }
        
        private void TotalVirtualCurrencySpentHandler(TotalVirtualCurrencySpentSignal obj)
        {
            foreach (var analytic in this.analyticEventList)
            {
                this.analyticServices.UserProperties[analytic.TotalVirtualCurrencySpentProperty] = obj.Amount;
                this.Track(analytic.TotalVirtualCurrencySpent(obj.CurrencyName, obj.Amount));
            }
        }
        
        private void TotalVirtualCurrencyEarnedHandler(TotalVirtualCurrencyEarnedSignal obj)
        {
            foreach (var analytic in this.analyticEventList)
            {
                this.analyticServices.UserProperties[analytic.TotalVirtualCurrencyEarnedProperty] = obj.Amount;
                this.Track(analytic.TotalVirtualCurrencyEarned(obj.CurrencyName, obj.Amount));
            }
        }
        
        private void DaysPlayedHandler(DaysPlayedSignal obj)
        {
            foreach (var analytic in this.analyticEventList)
            {
                this.analyticServices.UserProperties[analytic.DaysPlayedProperty] = obj.Days;
                this.Track(analytic.DaysPlayed(obj.Days));
            }
        }

        public void Dispose()
        {
            this.signalBus.Unsubscribe<LevelStartedSignal>(this.LevelStartedHandler);
            this.signalBus.Unsubscribe<LevelEndedSignal>(this.LevelEndedHandler);
            this.signalBus.Unsubscribe<LevelSkippedSignal>(this.LevelSkippedHandler);
            this.signalBus.Unsubscribe<TotalVirtualCurrencyEarnedSignal>(this.TotalVirtualCurrencyEarnedHandler);
            this.signalBus.Unsubscribe<TotalVirtualCurrencySpentSignal>(this.TotalVirtualCurrencySpentHandler);
            this.signalBus.Unsubscribe<DaysPlayedSignal>(this.DaysPlayedHandler);
            this.signalBus.Unsubscribe<InterstitialAdEligibleSignal>(this.InterstitialAdEligibleHandler);
            this.signalBus.Unsubscribe<InterstitialAdCalledSignal>(this.InterstitialAdCalledHandler);
            this.signalBus.Unsubscribe<InterstitialAdClickedSignal>(this.InterstitialAdClickedHandler);
            this.signalBus.Unsubscribe<InterstitialAdDownloadedSignal>(this.InterstitialAdLoadedHandler);
            this.signalBus.Unsubscribe<InterstitialAdLoadFailedSignal>(this.InterstitialAdFailedHandler);
            this.signalBus.Unsubscribe<InterstitialAdDisplayedSignal>(this.InterstitialAdDisplayedHandler);
            this.signalBus.Unsubscribe<RewardedInterstitialAdDisplayedSignal>(this.RewardedInterstitialAdDisplayedHandler);
            this.signalBus.Unsubscribe<RewardedAdOfferSignal>(this.RewardedAdOfferHandler);
            this.signalBus.Unsubscribe<RewardedAdEligibleSignal>(this.RewardedAdEligibleHandler);
            this.signalBus.Unsubscribe<RewardedAdCalledSignal>(this.RewardedAdCalledHandler);
            this.signalBus.Unsubscribe<RewardedAdLoadClickedSignal>(this.RewardedAdClickedHandler);
            this.signalBus.Unsubscribe<RewardedAdDisplayedSignal>(this.RewardedAdDisplayedHandler);
            this.signalBus.Unsubscribe<RewardedAdLoadFailedSignal>(this.RewardedAdFailedHandler);
            this.signalBus.Unsubscribe<RewardedAdLoadedSignal>(this.RewardedAdDownloadedHandler);
            this.signalBus.Unsubscribe<PopupShowedSignal>(this.PopupShowedHandler);

            this.adServices.RewardedAdCompleted     -= this.RewardedAdCompletedHandler;
            this.adServices.RewardedAdSkipped       -= this.RewardedAdSkippedHandler;
            this.adServices.InterstitialAdCompleted -= this.InterstitialAdCompletedHandler;
        }
    }
}