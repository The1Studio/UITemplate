#if ZITGA

namespace TheOneStudio.UITemplate.UITemplate.ThirdPartyServices.AnalyticEvents.Zitga
{
    using Core.AnalyticServices;
    using Zenject;

    public class ZitgaAnalyticEventFactory : BaseAnalyticEventFactory
    {
        public ZitgaAnalyticEventFactory(SignalBus signalBus, IAnalyticServices analyticServices) : base(signalBus, analyticServices)
        {
        }
    }
}

#endif