namespace TheOneStudio.UITemplate.UITemplate.ThirdPartyServices.AnalyticEvents.Sonat
{
    using Core.AnalyticServices.Data;
    using UnityEngine.Scripting;

    internal sealed class EndLevel : IEvent
    {
        public string Mode        { get; }
        public string Level       { get; }
        public int    StartCount  { get; }
        public int    PlayTime    { get; }
        public bool   IsFirstPlay { get; }
        public bool   Success     { get; }
        public string LoseCause   { get; }
        public string Flow        { get; }

        [Preserve]
        public EndLevel(string mode, string level, int startCount, int playTime, bool isFirstPlay, bool success, string loseCause, string flow)
        {
            this.Mode        = mode;
            this.Level       = level;
            this.StartCount  = startCount;
            this.PlayTime    = playTime;
            this.IsFirstPlay = isFirstPlay;
            this.Success     = success;
            this.LoseCause   = loseCause;
            this.Flow        = flow;
        }
    }
}
