namespace TheOneStudio.UITemplate.UITemplate.ThirdPartyServices.AnalyticEvents.Sonat
{
    using Core.AnalyticServices.Data;
    using UnityEngine.Scripting;

    [Preserve]
    internal sealed class StartLevel : IEvent
    {
        public string Mode        { get; }
        public string Level       { get; }
        public int    StartCount  { get; }
        public bool   IsFirstPlay { get; }

        public StartLevel(string mode, string level, int startCount, bool isFirstPlay)
        {
            this.Mode        = mode;
            this.Level       = level;
            this.StartCount  = startCount;
            this.IsFirstPlay = isFirstPlay;
        }
    }
}