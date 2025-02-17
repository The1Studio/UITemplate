namespace TheOneStudio.UITemplate.UITemplate.Services.AnalyticHandler
{
    #if BRAVESTARS

    using Core.AdsServices.Signals;
    using Core.AnalyticServices;
    using Core.AnalyticServices.CommonEvents;
    using GameFoundation.Scripts.UIModule.ScreenFlow.Managers;
    using GameFoundation.Signals;
    using TheOneStudio.UITemplate.UITemplate.Models.Controllers;
    using TheOneStudio.UITemplate.UITemplate.Signals;
    using TheOneStudio.UITemplate.UITemplate.ThirdPartyServices.AnalyticEvents;
    using TheOneStudio.UITemplate.UITemplate.ThirdPartyServices.AnalyticEvents.BraveStars;
    using TheOneStudio.UITemplate.UITemplate.ThirdPartyServices.AnalyticEvents.CommonEvents;
    using UnityEngine.Scripting;

    public class BraveStarsAnalyticHandler : UITemplateAnalyticHandler
    {
        protected override void InterstitialAdDisplayedHandler(InterstitialAdDisplayedSignal obj)
        {
            base.InterstitialAdDisplayedHandler(obj);

            if(this.uiTemplateAdsController.WatchInterstitialAds > 20) return;
            this.Track(new CustomEvent { EventName = $"af_inters_displayed_{this.uiTemplateAdsController.WatchInterstitialAds}_times" });
        }

        protected override void LevelEndedHandler(LevelEndedSignal obj)
        {
            base.LevelEndedHandler(obj);
            if (obj.IsWin)
            {
                this.analyticServices.Track(new AchievedLevel(obj.Level));
                this.analyticServices.Track(new CustomEvent
                {
                    EventName = $"completed_level_{obj.Level}",
                });
            }
        }

        #region Inject

        private readonly UITemplateAdsController uiTemplateAdsController;

        [Preserve]
        public BraveStarsAnalyticHandler(
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
    #endif

}