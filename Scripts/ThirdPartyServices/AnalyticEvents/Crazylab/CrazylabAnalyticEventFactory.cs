#if CRAZYLAB
namespace TheOneStudio.UITemplate.UITemplate.ThirdPartyServices.AnalyticEvents.Crazylab
{
    using System;
    using System.Collections.Generic;
    using Core.AnalyticServices;
    using TheOneStudio.UITemplate.UITemplate.ThirdPartyServices.AnalyticEvents;
    using GameFoundation.Signals;
    using TheOneStudio.UITemplate.UITemplate.Models.Controllers;
    using UnityEngine.Scripting;

    public class CrazylabAnalyticEventFactory : BaseAnalyticEventFactory
    {
        [Preserve]
        public CrazylabAnalyticEventFactory(SignalBus signalBus, IAnalyticServices analyticServices, UITemplateLevelDataController levelDataController) : base(signalBus, analyticServices, levelDataController)
        {
        }
    }
}
#endif