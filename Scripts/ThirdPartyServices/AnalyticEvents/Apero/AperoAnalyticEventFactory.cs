#if APERO
namespace TheOneStudio.UITemplate.UITemplate.ThirdPartyServices.AnalyticEvents.Apero
{

    using Core.AnalyticServices;
    using Core.AnalyticServices.Data;
    using GameFoundation.Signals;
    using TheOneStudio.UITemplate.UITemplate.Models.Controllers;
    using UnityEngine.Scripting;

    public class AperoAnalyticEventFactory : BaseAnalyticEventFactory
    {
        [Preserve]
        public AperoAnalyticEventFactory(SignalBus signalBus, IAnalyticServices analyticServices, UITemplateLevelDataController levelDataController) 
            : base(signalBus, analyticServices, levelDataController) { }

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