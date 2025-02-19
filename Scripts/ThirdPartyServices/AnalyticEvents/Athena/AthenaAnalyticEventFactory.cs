#if ATHENA
namespace TheOneStudio.UITemplate.UITemplate.ThirdPartyServices.AnalyticEvents.Athena
{
    using Core.AnalyticServices;
    using GameFoundation.Signals;

    public class AthenaAnalyticEventFactory : BaseAnalyticEventFactory
    {
        public AthenaAnalyticEventFactory(SignalBus signalBus, IAnalyticServices analyticServices) : base(signalBus, analyticServices)
        {
        }
    }
}
#endif