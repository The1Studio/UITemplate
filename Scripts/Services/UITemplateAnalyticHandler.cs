namespace TheOneStudio.UITemplate.UITemplate.Services
{
    using System;
    using System.Collections.Generic;
    using Core.AnalyticServices;
    using Core.AnalyticServices.Data;
    using TheOneStudio.UITemplate.UITemplate.Models;
    using TheOneStudio.UITemplate.UITemplate.Signals;
    using TheOneStudio.UITemplate.UITemplate.ThirdPartyServices.AnalyticEvents;
    using Zenject;

    public class UITemplateAnalyticHandler : IInitializable, IDisposable
    {
        #region inject

        private readonly SignalBus               signalBus;
        private readonly List<IAnalyticServices> analyticServices;
        private readonly IAnalyticEventFactory   analyticEventFactory;
        private readonly UITemplateUserLevelData uiTemplateUserLevelData;

        #endregion

        public UITemplateAnalyticHandler(SignalBus signalBus, List<IAnalyticServices> analyticServices, IAnalyticEventFactory analyticEventFactory, UITemplateUserLevelData uiTemplateUserLevelData)
        {
            this.signalBus               = signalBus;
            this.analyticServices        = analyticServices;
            this.analyticEventFactory    = analyticEventFactory;
            this.uiTemplateUserLevelData = uiTemplateUserLevelData;
        }

        public void Initialize()
        {
            this.analyticServices.ForEach(analytic => analytic.Start());

            this.signalBus.Subscribe<LevelStartedSignal>(this.LevelStartedHandler);
            this.signalBus.Subscribe<LevelEndedSignal>(this.LevelEndedHandler);
            this.signalBus.Subscribe<LevelSkippedSignal>(this.LevelSkippedHandler);
            this.signalBus.Subscribe<InterstitialAdShowedSignal>(this.InterstitialAdShowedHandler);
            this.signalBus.Subscribe<RewardedAdShowedSignal>(this.RewardedAdShowedHandler);
        }
        private void RewardedAdShowedHandler(RewardedAdShowedSignal obj) { this.AnalyticTrack(this.analyticEventFactory.RewardedVideoShow(this.uiTemplateUserLevelData.CurrentLevel, obj.place)); }
        private void InterstitialAdShowedHandler(InterstitialAdShowedSignal obj)
        {
            this.AnalyticTrack(this.analyticEventFactory.InterstitialShow(this.uiTemplateUserLevelData.CurrentLevel, obj.place));
        }

        private void AnalyticTrack(IEvent analyticEvent) { this.analyticServices.ForEach(analyticService => analyticService.Track(analyticEvent)); }

        private void LevelSkippedHandler(LevelSkippedSignal obj) { this.AnalyticTrack(this.analyticEventFactory.LevelSkipped(obj.Level, obj.Time)); }
        private void LevelEndedHandler(LevelEndedSignal obj)
        {
            this.AnalyticTrack(obj.IsWin ? this.analyticEventFactory.LevelWin(obj.Level, obj.Time) : this.analyticEventFactory.LevelLose(obj.Level, obj.Time));
        }
        private void LevelStartedHandler(LevelStartedSignal obj) { this.AnalyticTrack(this.analyticEventFactory.LevelStart(obj.Level)); }

        public void Dispose()
        {
            this.signalBus.Unsubscribe<LevelStartedSignal>(this.LevelStartedHandler);
            this.signalBus.Unsubscribe<LevelEndedSignal>(this.LevelEndedHandler);
            this.signalBus.Unsubscribe<LevelSkippedSignal>(this.LevelSkippedHandler);
        }
    }
}