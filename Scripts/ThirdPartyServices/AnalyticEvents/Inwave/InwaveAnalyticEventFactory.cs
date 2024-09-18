#if INWAVE
namespace TheOneStudio.UITemplate.UITemplate.ThirdPartyServices.AnalyticEvents.Inwave
{
    using Core.AnalyticServices;
    using Zenject;

    public class InwaveAnalyticEventFactory : BaseAnalyticEventFactory
    {
        public InwaveAnalyticEventFactory(SignalBus signalBus, IAnalyticServices analyticServices) : base(signalBus, analyticServices) { }
    }
}
#endif