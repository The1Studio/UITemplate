#if ATHENA
namespace TheOneStudio.UITemplate.UITemplate.ThirdPartyServices.AnalyticEvents.Athena
{
    using Core.AnalyticServices;
    using Core.AnalyticServices.CommonEvents;
    using Core.AnalyticServices.Data;
    using GameFoundation.Signals;
    using TheOneStudio.UITemplate.UITemplate.Models.Controllers;

    public class AthenaAnalyticEventFactory : BaseAnalyticEventFactory
    {
        public AthenaAnalyticEventFactory(SignalBus signalBus, IAnalyticServices analyticServices, UITemplateLevelDataController levelDataController) : base(signalBus, analyticServices, levelDataController)
        {
        }

        public override AnalyticsEventCustomizationConfig FireBaseAnalyticsEventCustomizationConfig { get; set; } = new()
        {
            IgnoreEvents = new(),
            CustomEventKeys = new()
                              {
                                  { nameof(AdsRevenueEvent), "ad_impression" },
                                  { "AdsRevenueSourceId", "ad_platform" },
                                  { "AdNetwork", "ad_source" },
                                  { "AdUnit", "ad_platform_unit_id" },
                                  { "NetworkPlacement", "ad_source_unit_id" },
                                  { "AdFormat", "ad_format" },
                                  { "Placement", "ad_placement" }
                              },
        };
    }
}
#endif