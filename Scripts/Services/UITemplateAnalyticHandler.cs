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
    using TheOneStudio.UITemplate.UITemplate.Models.Controllers;
    using TheOneStudio.UITemplate.UITemplate.Scripts.Signals;
    using TheOneStudio.UITemplate.UITemplate.Signals;
    using TheOneStudio.UITemplate.UITemplate.ThirdPartyServices.AnalyticEvents;
    using UnityEngine;
    using Zenject;

    public class UITemplateAnalyticHandler : IInitializable, IDisposable
    {
        #region inject

        private readonly SignalBus                         signalBus;
        private readonly IAnalyticServices                 analyticServices;
        private readonly List<IAnalyticEventFactory>       analyticEventList;
        private readonly IAdServices                       adServices;
        private readonly ILogService                       logService;
        private readonly UITemplateLevelDataController     uiTemplateLevelDataController;
        private readonly UITemplateInventoryDataController uITemplateInventoryDataController;
        private readonly UITemplateDailyRewardController   uiTemplateDailyRewardController;

        #endregion

        public UITemplateAnalyticHandler(SignalBus signalBus, IAnalyticServices analyticServices, List<IAnalyticEventFactory> analyticEventList,
            IAdServices adServices, ILogService logService, UITemplateLevelDataController uiTemplateLevelDataController, UITemplateInventoryDataController uITemplateInventoryDataController,
            UITemplateDailyRewardController uiTemplateDailyRewardController)
        {
            this.signalBus                         = signalBus;
            this.analyticServices                  = analyticServices;
            this.analyticEventList                 = analyticEventList;
            this.adServices                        = adServices;
            this.logService                        = logService;
            this.uiTemplateLevelDataController     = uiTemplateLevelDataController;
            this.uITemplateInventoryDataController = uITemplateInventoryDataController;
            this.uiTemplateDailyRewardController   = uiTemplateDailyRewardController;

            switch (analyticEventList.Count)
            {
                case > 1:
                    throw new Exception("Error: More than one analytic event factory found. Please remove one of them (Project Setting/Script Define Symbols).");
                case 0:
                    throw new Exception("Error: No analytic event factory found. Please add one of them (WIDO,ROCKET,ADONE,ABI...) into (Project Setting/Script Define Symbols).");
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
            this.signalBus.Subscribe<UpdateCurrencySignal>(this.UpdateCurrencyHandler);
            this.signalBus.Subscribe<InterstitialAdEligibleSignal>(this.InterstitialAdEligibleHandler);
            this.signalBus.Subscribe<InterstitialAdCalledSignal>(this.InterstitialAdCalledHandler);
            this.signalBus.Subscribe<InterstitialAdClickedSignal>(this.InterstitialAdClickedHandler);
            this.signalBus.Subscribe<InterstitialAdDownloadedSignal>(this.InterstitialAdLoadedHandler);
            this.signalBus.Subscribe<InterstitialAdLoadFailedSignal>(this.InterstitialAdFailedHandler);
            this.signalBus.Subscribe<InterstitialAdDisplayedSignal>(this.InterstitialAdDisplayedHandler);

            this.signalBus.Subscribe<RewardedInterstitialAdCompletedSignal>(this.RewardedInterstitialAdDisplayedHandler);
            this.signalBus.Subscribe<RewardedAdOfferSignal>(this.RewardedAdOfferHandler);
            this.signalBus.Subscribe<RewardedAdEligibleSignal>(this.RewardedAdEligibleHandler);
            this.signalBus.Subscribe<RewardedAdCalledSignal>(this.RewardedAdCalledHandler);
            this.signalBus.Subscribe<RewardedAdLoadClickedSignal>(this.RewardedAdClickedHandler);
            this.signalBus.Subscribe<RewardedAdDisplayedSignal>(this.RewardedAdDisplayedHandler);
            this.signalBus.Subscribe<RewardedAdLoadFailedSignal>(this.RewardedAdFailedHandler);
            this.signalBus.Subscribe<RewardedAdLoadedSignal>(this.RewardedAdDownloadedHandler);

            this.signalBus.Subscribe<PopupShowedSignal>(this.PopupShowedHandler);
            this.signalBus.Subscribe<UITemplateUnlockBuildingSignal>(this.UnlockBuildingHandler);

            //Ads events
            this.signalBus.Subscribe<InterstitialAdClosedSignal>(this.OnInterstitialAdClosed);
            this.signalBus.Subscribe<RewardedAdCompletedSignal>(this.OnRewardedAdCompleted);
            this.signalBus.Subscribe<RewardedSkippedSignal>(this.OnRewardedAdSkipped);

            this.TotalDaysPlayedChange();
        }

        private void OnRewardedAdSkipped(RewardedSkippedSignal obj)
        {
            foreach (var analytic in this.analyticEventList)
            {
                this.Track(analytic.RewardedVideoShowCompleted(0, obj.Placement, false));
            }
        }

        private void OnRewardedAdCompleted(RewardedAdCompletedSignal obj)
        {
            foreach (var analytic in this.analyticEventList)
            {
                this.Track(analytic.RewardedVideoShowCompleted(0, obj.Placement, true));
            }
        }

        private void OnInterstitialAdClosed(InterstitialAdClosedSignal obj)
        {
            foreach (var analytic in this.analyticEventList)
            {
                this.Track(analytic.InterstitialShowCompleted(0, obj.Placement));
            }
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

        private void InterstitialAdDisplayedHandler(InterstitialAdDisplayedSignal obj)
        {
            foreach (var analytic in this.analyticEventList)
            {
                this.analyticServices.UserProperties[analytic.LastAdsPlacementProperty]     = obj.Placement;
                this.analyticServices.UserProperties[analytic.TotalInterstitialAdsProperty] = obj.Placement;
                this.Track(analytic.InterstitialShow(this.uiTemplateLevelDataController.GetCurrentLevelData.Level, obj.Placement));
            }
        }

        private void InterstitialAdCalledHandler(InterstitialAdCalledSignal obj)
        {
            foreach (var analytic in this.analyticEventList)
            {
                this.Track(analytic.InterstitialCalled(obj.place));
            }
        }

        private void RewardedInterstitialAdDisplayedHandler(RewardedInterstitialAdCompletedSignal obj)
        {
            foreach (var analytic in this.analyticEventList)
            {
                this.analyticServices.UserProperties[analytic.LastAdsPlacementProperty] = obj.Placement;
                this.analyticServices.UserProperties[analytic.TotalRewardedAdsProperty] = obj.Placement;
                this.Track(analytic.RewardedInterstitialAdDisplayed(this.uiTemplateLevelDataController.GetCurrentLevelData.Level, obj.Placement));
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
                this.Track(analytic.RewardedVideoShow(this.uiTemplateLevelDataController.GetCurrentLevelData.Level, obj.Placement));
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
                    this.Track(analytic.FirstWin(obj.Level, obj.Time));
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
                this.analyticServices.UserProperties[analytic.LastLevelProperty] = this.uiTemplateLevelDataController.GetCurrentLevelData.Level;
                this.Track(analytic.LevelStart(obj.Level, this.uITemplateInventoryDataController.GetCurrencyValue()));
            }
        }

        private void UpdateCurrencyHandler(UpdateCurrencySignal obj)
        {
            foreach (var analytic in this.analyticEventList)
            {
                if (obj.Amount > 0)
                    this.analyticServices.UserProperties[analytic.TotalVirtualCurrencyEarnedProperty] = this.uITemplateInventoryDataController.GetCurrencyData(obj.Id).TotalEarned;
                else
                    this.analyticServices.UserProperties[analytic.TotalVirtualCurrencySpentProperty] =
                        this.uITemplateInventoryDataController.GetCurrencyData(obj.Id).TotalEarned - this.uITemplateInventoryDataController.GetCurrencyData(obj.Id).Value;
            }
        }

        private void TotalDaysPlayedChange()
        {
            foreach (var analytic in this.analyticEventList)
            {
                this.analyticServices.UserProperties[analytic.DaysPlayedProperty] = (int)(DateTime.Now.Date - this.uiTemplateDailyRewardController.GetFirstTimeOpenedDate.Date).TotalDays;
            }
        }

        private void UnlockBuildingHandler(UITemplateUnlockBuildingSignal obj)
        {
            foreach (var analytic in this.analyticEventList)
            {
                this.Track(analytic.BuildingUnlock(obj.IsUnlockSuccess));
            }   
        }

        public void Dispose()
        {
            this.signalBus.Unsubscribe<LevelStartedSignal>(this.LevelStartedHandler);
            this.signalBus.Unsubscribe<LevelEndedSignal>(this.LevelEndedHandler);
            this.signalBus.Unsubscribe<LevelSkippedSignal>(this.LevelSkippedHandler);
            this.signalBus.Unsubscribe<UpdateCurrencySignal>(this.UpdateCurrencyHandler);
            this.signalBus.Unsubscribe<InterstitialAdEligibleSignal>(this.InterstitialAdEligibleHandler);
            this.signalBus.Unsubscribe<InterstitialAdCalledSignal>(this.InterstitialAdCalledHandler);
            this.signalBus.Unsubscribe<InterstitialAdClickedSignal>(this.InterstitialAdClickedHandler);
            this.signalBus.Unsubscribe<InterstitialAdDownloadedSignal>(this.InterstitialAdLoadedHandler);
            this.signalBus.Unsubscribe<InterstitialAdLoadFailedSignal>(this.InterstitialAdFailedHandler);
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
            this.signalBus.Unsubscribe<UITemplateUnlockBuildingSignal>(this.UnlockBuildingHandler);
        }
    }
}