#if ATHENA
namespace TheOneStudio.UITemplate.UITemplate.ThirdPartyServices.AnalyticEvents.Athena
{
    using Core.AnalyticServices;
    using Core.AnalyticServices.CommonEvents;
    using Core.AnalyticServices.Signal;
    using GameFoundation.Signals;
    using TheOneStudio.UITemplate.UITemplate.Models.Controllers;
    using UnityEngine.Scripting;

    public class AthenaAnalyticEventFactory : BaseAnalyticEventFactory
    {
        [Preserve]
        public AthenaAnalyticEventFactory(SignalBus signalBus, IAnalyticServices analyticServices, UITemplateLevelDataController levelDataController) : base(signalBus, analyticServices, levelDataController)
        {
        }

        protected override void OnAdsRevenue(AdRevenueSignal obj)
        {
            this.analyticServices.Track(new CustomEvent
                                        {
                                            EventName = "bi_ad_value",
                                            EventProperties = new()
                                                              {
                                                                  { "ad_platform", obj.AdsRevenueEvent.AdsRevenueSourceId },
                                                                  { "ad_source", obj.AdsRevenueEvent.AdNetwork },
                                                                  { "ad_platform_unit_id", obj.AdsRevenueEvent.AdUnit },
                                                                  { "ad_source_unit_id", obj.AdsRevenueEvent.NetworkPlacement },
                                                                  { "ad_format", obj.AdsRevenueEvent.AdFormat },
                                                                  { "ad_placement", obj.AdsRevenueEvent.Placement },
                                                                  { "estimate_value_currency", obj.AdsRevenueEvent.Currency },
                                                                  { "estimate_value", obj.AdsRevenueEvent.Revenue },
                                                                  { "estimate_value_in_usd", obj.AdsRevenueEvent.Revenue },
                                                                  { "level_id", string.Empty }
                                                              },
                                        });
            
            this.analyticServices.Track(new CustomEvent
                                        {
                                            EventName = "ad_impression",
                                            EventProperties = new()
                                                              {
                                                                  { "ad_platform", obj.AdsRevenueEvent.AdsRevenueSourceId },
                                                                  { "ad_source", obj.AdsRevenueEvent.AdNetwork },
                                                                  { "ad_platform_unit_id", obj.AdsRevenueEvent.AdUnit },
                                                                  { "ad_source_unit_id", obj.AdsRevenueEvent.NetworkPlacement },
                                                                  { "ad_format", obj.AdsRevenueEvent.AdFormat },
                                                                  { "ad_placement", obj.AdsRevenueEvent.Placement },
                                                              },
                                        });
        }
    }
}
#endif