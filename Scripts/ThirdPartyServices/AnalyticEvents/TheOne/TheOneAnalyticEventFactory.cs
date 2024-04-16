#if THEONE
namespace TheOneStudio.UITemplate.UITemplate.ThirdPartyServices.AnalyticEvents.TheOne
{
    using Core.AnalyticServices;
    using Zenject;

    public class TheOneAnalyticEventFactory : BaseAnalyticEventFactory
    {
        public TheOneAnalyticEventFactory(SignalBus signalBus, IAnalyticServices analyticServices) : base(signalBus, analyticServices)
        {
        }
    }
}
#endif