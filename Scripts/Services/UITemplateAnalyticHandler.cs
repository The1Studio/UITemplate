namespace TheOneStudio.UITemplate.UITemplate.Services
{
    using System;
    using Core.AdsServices;
    using Core.AnalyticServices;
    using Core.AnalyticServices.Data;
    using GameFoundation.Scripts.UIModule.ScreenFlow.Signals;
    using TheOneStudio.UITemplate.UITemplate.Models;
    using TheOneStudio.UITemplate.UITemplate.Signals;
    using TheOneStudio.UITemplate.UITemplate.ThirdPartyServices.AnalyticEvents;
    using Zenject;

    public class UITemplateAnalyticHandler : IInitializable, IDisposable
    {
        #region inject

        private readonly SignalBus               signalBus;
        private readonly IAnalyticServices       analyticServices;
        private readonly IAnalyticEventFactory   analyticEventFactory;
        private readonly UITemplateUserLevelData uiTemplateUserLevelData;
        private readonly IAdServices             adServices;

        #endregion

        public UITemplateAnalyticHandler(SignalBus signalBus, IAnalyticServices analyticServices, IAnalyticEventFactory analyticEventFactory, UITemplateUserLevelData uiTemplateUserLevelData, IAdServices adServices)
        {
            this.signalBus               = signalBus;
            this.analyticServices        = analyticServices;
            this.analyticEventFactory    = analyticEventFactory;
            this.uiTemplateUserLevelData = uiTemplateUserLevelData;
            this.adServices              = adServices;
        }
        
        private void Track(IEvent trackEvent)
        {
            this.analyticEventFactory.ForceUpdateAllProperties();
            this.analyticServices.Track(trackEvent);
        }

        public void Initialize()
        {
            this.analyticServices.Start();

            //Game events
            this.signalBus.Subscribe<LevelStartedSignal>(this.LevelStartedHandler);
            this.signalBus.Subscribe<LevelEndedSignal>(this.LevelEndedHandler);
            this.signalBus.Subscribe<LevelSkippedSignal>(this.LevelSkippedHandler);
            this.signalBus.Subscribe<InterstitialAdShowedSignal>(this.InterstitialAdShowedHandler);
            this.signalBus.Subscribe<RewardedAdShowedSignal>(this.RewardedAdShowedHandler);
            this.signalBus.Subscribe<PopupShowedSignal>(this.PopupShowedHandler);
            
            //Ads events
            this.adServices.RewardedAdCompleted += this.RewardedAdCompletedHandler;
            this.adServices.RewardedAdSkipped   += this.RewardedAdSkippedHandler;
            this.adServices.InterstitialAdCompleted += this.InterstitialAdCompletedHandler;
        }
        
        private void PopupShowedHandler(PopupShowedSignal obj)
        {
        }

        private void InterstitialAdCompletedHandler(InterstitialAdNetwork arg1, string arg2)
        {
            this.Track(this.analyticEventFactory.InterstitialShowCompleted(0, arg2));
        }
        
        private void RewardedAdSkippedHandler(InterstitialAdNetwork arg1, string arg2)
        {
            this.Track(this.analyticEventFactory.RewardedVideoShowCompleted(0, arg2, false));
        }
        
        private void RewardedAdCompletedHandler(InterstitialAdNetwork arg1, string arg2)
        {
            this.Track(this.analyticEventFactory.RewardedVideoShowCompleted(0, arg2, true));
        }
        
        private void RewardedAdShowedHandler(RewardedAdShowedSignal obj)
        {
            this.Track(this.analyticEventFactory.RewardedVideoShow(this.uiTemplateUserLevelData.CurrentLevel, obj.place));
        }
        
        private void InterstitialAdShowedHandler(InterstitialAdShowedSignal obj)
        {
            this.Track(this.analyticEventFactory.InterstitialShow(this.uiTemplateUserLevelData.CurrentLevel, obj.place));
        }

        private void LevelSkippedHandler(LevelSkippedSignal obj) { this.Track(this.analyticEventFactory.LevelSkipped(obj.Level, obj.Time)); }
        
        private void LevelEndedHandler(LevelEndedSignal obj)
        {
            this.Track(obj.IsWin ? this.analyticEventFactory.LevelWin(obj.Level, obj.Time) : this.analyticEventFactory.LevelLose(obj.Level, obj.Time));
        }
        
        private void LevelStartedHandler(LevelStartedSignal obj) { this.Track(this.analyticEventFactory.LevelStart(obj.Level)); }

        public void Dispose()
        {
            this.signalBus.Unsubscribe<LevelStartedSignal>(this.LevelStartedHandler);
            this.signalBus.Unsubscribe<LevelEndedSignal>(this.LevelEndedHandler);
            this.signalBus.Unsubscribe<LevelSkippedSignal>(this.LevelSkippedHandler);
            this.signalBus.Unsubscribe<InterstitialAdShowedSignal>(this.InterstitialAdShowedHandler);
            this.signalBus.Unsubscribe<RewardedAdShowedSignal>(this.RewardedAdShowedHandler);
            
            this.adServices.RewardedAdCompleted     -= this.RewardedAdCompletedHandler;
            this.adServices.RewardedAdSkipped       -= this.RewardedAdSkippedHandler;
            this.adServices.InterstitialAdCompleted -= this.InterstitialAdCompletedHandler;
        }
    }
}