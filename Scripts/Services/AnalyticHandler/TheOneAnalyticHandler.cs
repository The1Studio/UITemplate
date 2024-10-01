#if THEONE

namespace TheOneStudio.UITemplate.UITemplate.Services.AnalyticHandler
{
    using Core.AnalyticServices;
    using GameFoundation.Signals;
    using TheOneStudio.UITemplate.UITemplate.Models.Controllers;
    using TheOneStudio.UITemplate.UITemplate.Signals;
    using TheOneStudio.UITemplate.UITemplate.ThirdPartyServices.AnalyticEvents;
    using TheOneStudio.UITemplate.UITemplate.ThirdPartyServices.AnalyticEvents.CommonEvents;

    public class TheOneAnalyticHandler :UITemplateAnalyticHandler
    {
        public TheOneAnalyticHandler(SignalBus signalBus,
                                     IAnalyticServices analyticServices, 
                                     IAnalyticEventFactory analyticEventFactory, 
                                     UITemplateLevelDataController uiTemplateLevelDataController,
                                     UITemplateInventoryDataController uITemplateInventoryDataController, 
                                     UITemplateDailyRewardController uiTemplateDailyRewardController, 
                                     UITemplateGameSessionDataController uITemplateGameSessionDataController)
            : base(signalBus, analyticServices, analyticEventFactory, uiTemplateLevelDataController, uITemplateInventoryDataController, uiTemplateDailyRewardController, uITemplateGameSessionDataController)
        {
        }

        public override void Initialize()
        {
            // level
            this.signalBus.Subscribe<LevelEndedSignal>(this.HandleLevelEnd);
        }

        private void HandleLevelEnd(LevelEndedSignal signal)
        {
            this.Track(new LevelEnd(level: signal.Level,
                                    status: signal.EndStatus.ToString(),
                                    gameModeId: signal.GameModeId,
                                    timePlay: signal.Time,
                                    spentResources: signal.SpentResources,
                                    timestamp: signal.Timestamp));
        }
        public override void Dispose()
        {
            // level
            this.signalBus.Unsubscribe<LevelEndedSignal>(this.HandleLevelEnd);
        }
    }
}

#endif