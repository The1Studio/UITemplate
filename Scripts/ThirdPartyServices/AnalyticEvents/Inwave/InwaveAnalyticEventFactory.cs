#if INWAVE
namespace TheOneStudio.UITemplate.UITemplate.ThirdPartyServices.AnalyticEvents.Inwave
{
    using Core.AnalyticServices;
    using Zenject;
    using UnityEngine.Scripting;

    public class InwaveAnalyticEventFactory : BaseAnalyticEventFactory
    {
        [Preserve]
        public InwaveAnalyticEventFactory(SignalBus signalBus, IAnalyticServices analyticServices) : base(signalBus, analyticServices) { }
    }
}
#endif