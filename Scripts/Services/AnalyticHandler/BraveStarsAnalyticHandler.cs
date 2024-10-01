#if BRAVESTARS

namespace TheOneStudio.UITemplate.UITemplate.Services.AnalyticHandler
{
    using Core.AdsServices.Signals;
    using Core.AnalyticServices;
    using Core.AnalyticServices.CommonEvents;
    using GameFoundation.Signals;
    using TheOneStudio.UITemplate.UITemplate.Models.Controllers;
    using TheOneStudio.UITemplate.UITemplate.ThirdPartyServices.AnalyticEvents;
    using UnityEngine.Scripting;

    public class BraveStarsAnalyticHandler : UITemplateAnalyticHandler
    {
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
            UITemplateGameSessionDataController uITemplateGameSessionDataController
        ) : base(signalBus, analyticServices, analyticEventFactory, uiTemplateLevelDataController, uITemplateInventoryDataController, uiTemplateDailyRewardController, uITemplateGameSessionDataController)
        {
            this.uiTemplateAdsController = uiTemplateAdsController;
        }

        #endregion

        public override void Initialize()
        {
            base.Initialize();
            this.signalBus.Subscribe<InterstitialAdDisplayedSignal>(this.InterstitialAdDisplayedHandler);
        }

        private void InterstitialAdDisplayedHandler(InterstitialAdDisplayedSignal obj)
        {
            if(this.uiTemplateAdsController.WatchInterstitialAds >= 20) return;
            this.uiTemplateAdsController.UpdateWatchedInterstitialAds();
            this.Track(new CustomEvent { EventName = $"af_inters_displayed_{this.uiTemplateAdsController.WatchInterstitialAds}_times" });
        }

        public override void Dispose()
        {
            base.Dispose();
            this.signalBus.Unsubscribe<InterstitialAdDisplayedSignal>(this.InterstitialAdDisplayedHandler);
        }
    }
}

#endif