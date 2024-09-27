namespace TheOneStudio.UITemplate.UITemplate.ThirdPartyServices.AnalyticEvents.Falcon
{
    using System.Collections.Generic;
    using GameFoundation.Scripts.Interfaces;
    using UnityEngine.Scripting;

#if FALCON
    [Preserve]
    public class FalconLocalData : ILocalData
    {
        public void Init() { }

        public HashSet<int> PassedLevels { get; set; } = new();
    }
#endif
}