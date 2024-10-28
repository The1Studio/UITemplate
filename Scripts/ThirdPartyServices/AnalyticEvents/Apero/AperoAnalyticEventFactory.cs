#if APERO
namespace TheOneStudio.UITemplate.UITemplate.ThirdPartyServices.AnalyticEvents.Apero
{
    using Core.AnalyticServices;
    using Core.AnalyticServices.Data;
    using UnityEngine.Scripting;

    [Preserve]
    public class AperoAnalyticEventFactory : BaseAnalyticEventFactory
    {
        private readonly IAnalyticServices analyticServices;

        public override AnalyticsEventCustomizationConfig ByteBrewAnalyticsEventCustomizationConfig { get; set; } = new()
        {
            IgnoreEvents = new(),
            CustomEventKeys = new()
            {
                { nameof(Rev), "Rev" },
                { nameof(Purchase), "Purchase" },
            },
        };

        public override AnalyticsEventCustomizationConfig FireBaseAnalyticsEventCustomizationConfig { get; set; } = new()
        {
            IgnoreEvents = new(),
            CustomEventKeys = new()
            {
                { nameof(Rev), "Rev" },
                { nameof(Purchase), "Purchase" },
            },
        };
    }

}
#endif