namespace TheOneStudio.UITemplate.UITemplate.Services.AnalyticHandler.HiGame
{
#if HIGAME
    using System.Collections.Generic;
    using GameFoundation.Scripts.Interfaces;
    using UnityEngine.Scripting;

    [Preserve]
    public class HiGameLocalData : ILocalData
    {
        public void Init() { }

        public HashSet<int> PassedLevels { get; set; } = new();
    }
#endif
}