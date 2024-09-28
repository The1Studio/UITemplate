#if THEONE
namespace TheOneStudio.UITemplate.UITemplate.ThirdPartyServices.AnalyticEvents.TheOne
{
    using Core.AnalyticServices;
    using GameFoundation.Signals;
    using UnityEngine.Scripting;

    public class TheOneAnalyticEventFactory : BaseAnalyticEventFactory
    {
        [Preserve]
        public TheOneAnalyticEventFactory(SignalBus signalBus, IAnalyticServices analyticServices) : base(signalBus, analyticServices)
        {
        }
    }
}
#endif