namespace TheOneStudio.UITemplate.UITemplate.Services.AnalyticHandler
{
    using System;
    using Core.AnalyticServices;
    using Core.AnalyticServices.CommonEvents;
    using Core.AnalyticServices.Data;
    using GameFoundation.DI;
    using GameFoundation.Signals;
    using TheOneStudio.UITemplate.UITemplate.Models.Controllers;
    using TheOneStudio.UITemplate.UITemplate.Signals;
    using TheOneStudio.UITemplate.UITemplate.ThirdPartyServices.AnalyticEvents;
    using TheOneStudio.UITemplate.UITemplate.ThirdPartyServices.AnalyticEvents.TheOne;

    public class TheOneAnalyticHandler : IInitializable, IDisposable
    {
        private readonly SignalBus                         signalBus;
        private readonly IAnalyticEventFactory             analyticEventFactory;
        private readonly IAnalyticServices                 analyticServices;
        private readonly UITemplateInventoryDataController inventoryDataController;
        private readonly UITemplateLevelDataController     levelDataController;

        public TheOneAnalyticHandler(SignalBus                         signalBus,
                                     IAnalyticEventFactory             analyticEventFactory,
                                     IAnalyticServices                 analyticServices,
                                     UITemplateInventoryDataController inventoryDataController,
                                     UITemplateLevelDataController     levelDataController)
        {
            this.signalBus               = signalBus;
            this.analyticEventFactory    = analyticEventFactory;
            this.analyticServices        = analyticServices;
            this.inventoryDataController = inventoryDataController;
            this.levelDataController     = levelDataController;
        }

        public void Initialize()
        {
            // level
            this.signalBus.Subscribe<LevelStartedSignal>(this.HandleLevelStart);
            this.signalBus.Subscribe<LevelEndedSignal>(this.HandleLevelEnd);
        }

        private void HandleLevelStart(LevelStartedSignal signal)
        {
            this.Track(new LevelStart(level: signal.Level,
                                      gold: this.inventoryDataController.GetCurrencyValue(),
                                      totalLevelsPlayed: this.levelDataController.TotalLevelSurpassed,
                                      timestamp: DateTimeOffset.UtcNow.ToUnixTimeSeconds(),
                                      gameModeId: signal.GameModeId,
                                      totalLevelsTypePlayed: signal.TotalLevelsTypePlayed));
        }

        private void HandleLevelEnd(LevelEndedSignal signal)
        {
            this.Track(new LevelEnd(level: signal.Level,
                                    status: signal.EndStatus.ToString(),
                                    gameModeId: signal.GameModeId,
                                    timePlay: signal.Time,
                                    gainedRewards: signal.GainedRewards,
                                    spentResources: signal.SpentResources,
                                    timestamp: signal.Timestamp));
        }

        private void Track(IEvent trackEvent)
        {
            this.analyticEventFactory.ForceUpdateAllProperties();

            if (trackEvent is CustomEvent customEvent &&
                string.IsNullOrEmpty(customEvent.EventName))
                return;

            this.analyticServices.Track(trackEvent);
        }

        public void Dispose()
        {
            // level
            this.signalBus.Unsubscribe<LevelStartedSignal>(this.HandleLevelStart);
            this.signalBus.Unsubscribe<LevelEndedSignal>(this.HandleLevelEnd);
        }
    }
}