#if APERO
namespace TheOneStudio.UITemplate.UITemplate.Services.AnalyticHandler
{

    using Core.AdsServices;
    using Core.AdsServices.Signals;
    using Core.AnalyticServices;
    using Core.AnalyticServices.CommonEvents;
    using Core.AnalyticServices.Signal;
    using GameFoundation.Signals;
    using ServiceImplementation.IAPServices.Signals;
    using TheOneStudio.UITemplate.UITemplate.Models.Controllers;
    using TheOneStudio.UITemplate.UITemplate.ThirdPartyServices.AnalyticEvents;
    using TheOneStudio.UITemplate.UITemplate.ThirdPartyServices.AnalyticEvents.Apero;
    using UnityEngine.Scripting;

    public class AperoAnalyticHandler : UITemplateAnalyticHandler
    {
        [Preserve]
        public AperoAnalyticHandler(SignalBus signalBus, IAnalyticServices analyticServices, IAnalyticEventFactory analyticEventFactory, UITemplateLevelDataController uiTemplateLevelDataController,
                                    UITemplateInventoryDataController uITemplateInventoryDataController, UITemplateDailyRewardController uiTemplateDailyRewardController,
                                    UITemplateGameSessionDataController uITemplateGameSessionDataController) : base(signalBus, analyticServices, analyticEventFactory, uiTemplateLevelDataController,
                                                                                                                    uITemplateInventoryDataController, uiTemplateDailyRewardController,
                                                                                                                    uITemplateGameSessionDataController)
        {
        }

        public override void Initialize()
        {
            base.Initialize();
            this.signalBus.Subscribe<OnIAPPurchaseSuccessSignal>(this.OnIAPPurchaseSuccessHandler);
            this.signalBus.Subscribe<AdRevenueSignal>(this.OnAdRevenueHandler);
            this.signalBus.Subscribe<AdRequestSignal>(signal => this.TrackAdEvent("track_ad_request", signal.AdInfo));

            this.signalBus.Subscribe<BannerAdLoadedSignal>(signal => this.TrackAdEvent("track_ad_matched_request", signal.AdInfo));
            this.signalBus.Subscribe<BannerAdClickedSignal>(signal => this.TrackAdEvent("track_ad_click", signal.AdInfo));

            this.signalBus.Subscribe<InterstitialAdLoadedSignal>(signal => this.TrackAdEvent("track_ad_matched_request", signal.AdInfo));
            this.signalBus.Subscribe<InterstitialAdClickedSignal>(signal => this.TrackAdEvent("track_ad_click", signal.AdInfo));

            this.signalBus.Subscribe<RewardedAdLoadedSignal>(signal => this.TrackAdEvent("track_ad_matched_request", signal.AdInfo));
            this.signalBus.Subscribe<RewardedAdClickedSignal>(signal => this.TrackAdEvent("track_ad_click", signal.AdInfo));

            this.signalBus.Subscribe<AppOpenLoadedSignal>(signal => this.TrackAdEvent("track_ad_matched_request", signal.AdInfo));
            this.signalBus.Subscribe<AppOpenFullScreenContentClosedSignal>(signal => this.TrackAdEvent("track_ad_click", signal.AdInfo));
        }

        private void OnIAPPurchaseSuccessHandler(OnIAPPurchaseSuccessSignal signal) { this.analyticServices.Track(new Purchase(signal.Product)); }

        private void OnAdRevenueHandler(AdRevenueSignal signal)
        {
            this.TrackAdEvent("track_ad_impression",
                              new(signal.AdsRevenueEvent.AdsRevenueSourceId,
                                  signal.AdsRevenueEvent.AdUnit,
                                  signal.AdsRevenueEvent.AdFormat,
                                  signal.AdsRevenueEvent.AdNetwork,
                                  signal.AdsRevenueEvent.NetworkPlacement,
                                  signal.AdsRevenueEvent.Revenue,
                                  signal.AdsRevenueEvent.Currency));
        }

        private void TrackAdEvent(string eventName, AdInfo adInfo)
        {
            this.analyticServices.Track(new CustomEvent
            {
                EventName = eventName,
                EventProperties = new()
                {
                    { "ad_platform", adInfo.AdPlatform },
                    { "ad_unit_id", adInfo.AdUnitId },
                    { "ad_source", adInfo.AdSource },
                    { "ad_source_unit_id", adInfo.AdSourceUnitId },
                    { "ad_format", adInfo.AdFormat },
                    { "currency", adInfo.Currency },
                    { "value", adInfo.Value },
                },
            });
        }
    }

}
#endif