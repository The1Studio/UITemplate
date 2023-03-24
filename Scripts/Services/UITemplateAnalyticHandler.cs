namespace TheOneStudio.UITemplate.UITemplate.Services
{
    using System;
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
        private readonly IAnalyticEventFactory             analyticEventFactory;
        private readonly UITemplateUserLevelData           uiTemplateUserLevelData;
        private readonly IAdServices                       adServices;
        private readonly ILogService                       logService;
        private readonly UITemplateLevelDataController     uiTemplateLevelDataController;
        private readonly UITemplateInventoryDataController uITemplateInventoryDataController;

        #endregion

        public UITemplateAnalyticHandler(SignalBus signalBus, IAnalyticServices analyticServices, IAnalyticEventFactory analyticEventFactory, UITemplateUserLevelData uiTemplateUserLevelData,
            IAdServices adServices, ILogService logService, UITemplateLevelDataController uiTemplateLevelDataController, UITemplateInventoryDataController uITemplateInventoryDataController)
        {
            this.signalBus                         = signalBus;
            this.analyticServices                  = analyticServices;
            this.analyticEventFactory              = analyticEventFactory;
            this.uiTemplateUserLevelData           = uiTemplateUserLevelData;
            this.adServices                        = adServices;
            this.logService                        = logService;
            this.uiTemplateLevelDataController     = uiTemplateLevelDataController;
            this.uITemplateInventoryDataController = uITemplateInventoryDataController;
        }

        private void Track(IEvent trackEvent)
        {
            this.analyticEventFactory.ForceUpdateAllProperties();

            if (trackEvent is CustomEvent customEvent && string.IsNullOrEmpty(customEvent.EventName))
                return;
            this.analyticServices.Track(trackEvent);
        }

        public void Initialize()
        {
            this.analyticServices.Start();

            //Game events
            this.signalBus.Subscribe<LevelStartedSignal>(this.LevelStartedHandler);
            this.signalBus.Subscribe<LevelEndedSignal>(this.LevelEndedHandler);
            this.signalBus.Subscribe<LevelSkippedSignal>(this.LevelSkippedHandler);
            this.signalBus.Subscribe<InterstitialAdEligibleSignal>(this.InterstitialAdEligibleHandler);
            this.signalBus.Subscribe<InterstitialAdShowedSignal>(this.InterstitialAdShowedHandler);
            this.signalBus.Subscribe<InterstitialAdClickedSignal>(this.InterstitialAdClickedHandler);
            this.signalBus.Subscribe<InterstitialAdLoadedSignal>(this.InterstitialAdLoadedHandler);
            this.signalBus.Subscribe<InterstitialAdLoadFailedSignal>(this.InterstitialAdFailedHandler);
            this.signalBus.Subscribe<RewardedAdOfferSignal>(this.RewardedAdOfferHandler);
            this.signalBus.Subscribe<RewardedAdEligibleSignal>(this.RewardedAdEligibleHandler);
            this.signalBus.Subscribe<RewardedAdShowedSignal>(this.RewardedAdShowedHandler);
            this.signalBus.Subscribe<RewardedAdLoadClickedSignal>(this.RewardedAdClickedHandler);
            this.signalBus.Subscribe<RewardedAdLoadFailedSignal>(this.RewardedAdFailedHandler);
            this.signalBus.Subscribe<RewardedAdLoadedSignal>(this.RewardedAdLoadedHandler);
            this.signalBus.Subscribe<PopupShowedSignal>(this.PopupShowedHandler);

            //Ads events
            this.adServices.RewardedAdCompleted     += this.RewardedAdCompletedHandler;
            this.adServices.RewardedAdSkipped       += this.RewardedAdSkippedHandler;
            this.adServices.InterstitialAdCompleted += this.InterstitialAdCompletedHandler;
        }
        private void RewardedAdEligibleHandler(RewardedAdEligibleSignal obj)
        {
            this.Track(this.analyticEventFactory.RewardedVideoEligible(obj.place));
        }
        private void InterstitialAdEligibleHandler(InterstitialAdEligibleSignal obj)
        {
            this.Track(this.analyticEventFactory.InterstitialEligible(obj.place));
        }

        #region Interstitial Ads Signal Handler

        private void InterstitialAdClickedHandler(InterstitialAdClickedSignal obj) { this.Track(this.analyticEventFactory.InterstitialClick(obj.Placement)); }

        private void InterstitialAdLoadedHandler(InterstitialAdLoadedSignal obj) { this.Track(this.analyticEventFactory.InterstitialLoaded(obj.Placement)); }

        private void InterstitialAdFailedHandler(InterstitialAdLoadFailedSignal obj) { this.Track(this.analyticEventFactory.InterstitialShowFail(obj.Placement, obj.Message)); }

        private void InterstitialAdShowedHandler(InterstitialAdShowedSignal obj)
        {
            this.analyticServices.UserProperties[this.analyticEventFactory.LastAdsPlacementProperty]     = obj.place;
            this.analyticServices.UserProperties[this.analyticEventFactory.TotalInterstitialAdsProperty] = obj.place;
            this.Track(this.analyticEventFactory.InterstitialShow(this.uiTemplateUserLevelData.CurrentLevel, obj.place));
        }

        private void InterstitialAdCompletedHandler(InterstitialAdNetwork arg1, string arg2) { this.Track(this.analyticEventFactory.InterstitialShowCompleted(0, arg2)); }

        #endregion

        #region Rewarded Ads Signal Handler

        private void RewardedAdFailedHandler(RewardedAdLoadFailedSignal obj) { this.Track(this.analyticEventFactory.RewardedVideoShowFail(obj.Placement, obj.Message)); }

        private void RewardedAdOfferHandler(RewardedAdOfferSignal obj) { this.Track(this.analyticEventFactory.RewardedVideoOffer(obj.Place)); }
        
        private void RewardedAdLoadedHandler(RewardedAdLoadedSignal obj) { this.Track(this.analyticEventFactory.RewardedVideoLoaded(obj.Placement)); }

        private void RewardedAdClickedHandler(RewardedAdLoadClickedSignal obj) { this.Track(this.analyticEventFactory.RewardedVideoClick(obj.Placement)); }

        private void RewardedAdSkippedHandler(RewardedAdNetwork arg1, string arg2) { this.Track(this.analyticEventFactory.RewardedVideoShowCompleted(0, arg2, false)); }

        private void RewardedAdCompletedHandler(RewardedAdNetwork arg1, string arg2) { this.Track(this.analyticEventFactory.RewardedVideoShowCompleted(0, arg2, true)); }

        private void RewardedAdShowedHandler(RewardedAdShowedSignal obj)
        {
            this.analyticServices.UserProperties[this.analyticEventFactory.LastAdsPlacementProperty] = obj.place;
            this.analyticServices.UserProperties[this.analyticEventFactory.TotalRewardedAdsProperty] = obj.place;
            this.Track(this.analyticEventFactory.RewardedVideoShow(this.uiTemplateUserLevelData.CurrentLevel, obj.place));
        }

        #endregion

        private void PopupShowedHandler(PopupShowedSignal obj) { }

        private void LevelSkippedHandler(LevelSkippedSignal obj) { this.Track(this.analyticEventFactory.LevelSkipped(obj.Level, obj.Time)); }

        private void LevelEndedHandler(LevelEndedSignal obj)
        {
            this.analyticServices.UserProperties[this.analyticEventFactory.LevelMaxProperty] = this.uiTemplateLevelDataController.MaxLevel;
            var levelData = this.uiTemplateLevelDataController.GetLevelData(obj.Level);
            this.Track(obj.IsWin
                ? this.analyticEventFactory.LevelWin(obj.Level, obj.Time, levelData.WinCount)
                : this.analyticEventFactory.LevelLose(obj.Level, obj.Time, levelData.LoseCount));

            if (obj.IsWin && levelData.WinCount == 1)
            {
                this.analyticEventFactory.FirstWin(obj.Level, obj.Time);
            }

            if (!obj.IsWin)
            {
                this.analyticEventFactory.LevelLose(obj.Level, obj.Time, levelData.LoseCount);
            }
        }

        private void LevelStartedHandler(LevelStartedSignal obj)
        {
            this.analyticServices.UserProperties[this.analyticEventFactory.LastLevelProperty] = this.uiTemplateUserLevelData.CurrentLevel;
            this.Track(this.analyticEventFactory.LevelStart(obj.Level, this.uITemplateInventoryDataController.GetCurrency().Value));
        }

        public void Dispose()
        {
            this.signalBus.Unsubscribe<LevelStartedSignal>(this.LevelStartedHandler);
            this.signalBus.Unsubscribe<LevelEndedSignal>(this.LevelEndedHandler);
            this.signalBus.Unsubscribe<LevelSkippedSignal>(this.LevelSkippedHandler);
            this.signalBus.Unsubscribe<InterstitialAdEligibleSignal>(this.InterstitialAdEligibleHandler);
            this.signalBus.Unsubscribe<InterstitialAdShowedSignal>(this.InterstitialAdShowedHandler);
            this.signalBus.Unsubscribe<InterstitialAdClickedSignal>(this.InterstitialAdClickedHandler);
            this.signalBus.Unsubscribe<InterstitialAdLoadedSignal>(this.InterstitialAdLoadedHandler);
            this.signalBus.Unsubscribe<InterstitialAdLoadFailedSignal>(this.InterstitialAdFailedHandler);
            this.signalBus.Unsubscribe<RewardedAdOfferSignal>(this.RewardedAdOfferHandler);
            this.signalBus.Unsubscribe<RewardedAdEligibleSignal>(this.RewardedAdEligibleHandler);
            this.signalBus.Unsubscribe<RewardedAdShowedSignal>(this.RewardedAdShowedHandler);
            this.signalBus.Unsubscribe<RewardedAdLoadClickedSignal>(this.RewardedAdClickedHandler);
            this.signalBus.Unsubscribe<RewardedAdLoadFailedSignal>(this.RewardedAdFailedHandler);
            this.signalBus.Unsubscribe<RewardedAdLoadedSignal>(this.RewardedAdLoadedHandler);
            this.signalBus.Unsubscribe<PopupShowedSignal>(this.PopupShowedHandler);

            this.adServices.RewardedAdCompleted     -= this.RewardedAdCompletedHandler;
            this.adServices.RewardedAdSkipped       -= this.RewardedAdSkippedHandler;
            this.adServices.InterstitialAdCompleted -= this.InterstitialAdCompletedHandler;
        }
    }
}