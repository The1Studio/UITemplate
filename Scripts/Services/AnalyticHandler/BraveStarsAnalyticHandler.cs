namespace TheOneStudio.UITemplate.UITemplate.Services.AnalyticHandler
{
    using System.Collections.Generic;
    using Core.AdsServices.Signals;
    using Core.AnalyticServices;
    using Core.AnalyticServices.CommonEvents;
    using TheOneStudio.UITemplate.UITemplate.Models.Controllers;
    using TheOneStudio.UITemplate.UITemplate.ThirdPartyServices.AnalyticEvents;
    using Zenject;

    public class BraveStarsAnalyticHandler : UITemplateAnalyticHandler
    {
        #region Inject

        private readonly UITemplateAdsController uiTemplateAdsController;
        
        public BraveStarsAnalyticHandler(SignalBus                         signalBus, 
                                         IAnalyticServices                 analyticServices, 
                                         List<IAnalyticEventFactory>       analyticEventFactories, 
                                         UITemplateLevelDataController     uiTemplateLevelDataController, 
                                         UITemplateInventoryDataController uITemplateInventoryDataController, 
                                         UITemplateDailyRewardController   uiTemplateDailyRewardController,
                                         UITemplateAdsController           uiTemplateAdsController,
                                         UITemplateGameSessionDataController uITemplateGameSessionDataController) : base(signalBus, analyticServices, analyticEventFactories, uiTemplateLevelDataController, uITemplateInventoryDataController, uiTemplateDailyRewardController, uITemplateGameSessionDataController)
        {
            this.uiTemplateAdsController = uiTemplateAdsController;
        }

        #endregion

        protected override void InterstitialAdDisplayedHandler(InterstitialAdDisplayedSignal obj)
        {
            base.InterstitialAdDisplayedHandler(obj);
            
#if BRAVESTARS
            if(this.uiTemplateAdsController.WatchInterstitialAds >= 20) return;
            this.uiTemplateAdsController.UpdateWatchedInterstitialAds();
            this.Track(new CustomEvent { EventName = $"af_inters_displayed_{this.uiTemplateAdsController.WatchInterstitialAds}_times" });
#endif
        }
    }
}