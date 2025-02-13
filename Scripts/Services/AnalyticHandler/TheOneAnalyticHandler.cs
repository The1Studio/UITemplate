#if THEONE
namespace TheOneStudio.UITemplate.UITemplate.Services.AnalyticHandler
{
    using Core.AnalyticServices;
    using Core.AnalyticServices.CommonEvents;
    using Core.AnalyticServices.Signal;
    using GameFoundation.Scripts.UIModule.ScreenFlow.Managers;
    using GameFoundation.Signals;
    using TheOneStudio.UITemplate.UITemplate.Models.Controllers;
    using TheOneStudio.UITemplate.UITemplate.ThirdPartyServices.AnalyticEvents;
    using UnityEngine.Scripting;

    public class TheOneAnalyticHandler : UITemplateAnalyticHandler
    {
        protected override void AddRevenueHandler(AdRevenueSignal obj)
        {
            this.analyticServices.Track(new CustomEvent
            {
                EventName = "ad_revenue_sdk",
                EventProperties = new()
                {
                    { "play_mode", "classic" },
                    { "level", this.uiTemplateLevelDataController.CurrentLevel },
                    { "ad_format", obj.AdsRevenueEvent.AdFormat },
                    { "value", obj.AdsRevenueEvent.Revenue },
                    { "location", obj.AdsRevenueEvent.Placement },
                    { "ad_network", obj.AdsRevenueEvent.AdNetwork },
                    { "ad_platform", obj.AdsRevenueEvent.AdsRevenueSourceId },
                    { "ad_unit_name", obj.AdsRevenueEvent.AdUnit },
                    { "currency", obj.AdsRevenueEvent.Currency },
                },
            });
        }

        #region Inject

        private readonly UITemplateAdsController uiTemplateAdsController;

        [Preserve]
        public TheOneAnalyticHandler(
            SignalBus                           signalBus,
            IAnalyticServices                   analyticServices,
            IAnalyticEventFactory               analyticEventFactory,
            UITemplateLevelDataController       uiTemplateLevelDataController,
            UITemplateInventoryDataController   uITemplateInventoryDataController,
            UITemplateDailyRewardController     uiTemplateDailyRewardController,
            UITemplateAdsController             uiTemplateAdsController,
            UITemplateGameSessionDataController uITemplateGameSessionDataController,
            IScreenManager                      screenManager
        ) : base(signalBus, analyticServices, analyticEventFactory, uiTemplateLevelDataController, uITemplateInventoryDataController, uiTemplateDailyRewardController, uITemplateGameSessionDataController, screenManager)
        {
            this.uiTemplateAdsController = uiTemplateAdsController;
        }

        #endregion
    }
}
#endif