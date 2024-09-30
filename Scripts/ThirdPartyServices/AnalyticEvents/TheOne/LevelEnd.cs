namespace TheOneStudio.UITemplate.UITemplate.ThirdPartyServices.AnalyticEvents.TheOne
{
    using System.Collections.Generic;
    using Core.AnalyticServices.Data;

    public class LevelEnd : IEvent
    {
        public int                        Level          { get; set; }
        public string                     Status         { get; set; }
        public int                        GameModeId     { get; set; }
        public long                       TimePlay       { get; set; }
        public Dictionary<string, object> GainedRewards  { get; set; }
        public Dictionary<string, object> spentResources { get; set; }
        public long?                      Timestamp      { get; set; }

        public LevelEnd(int level, string status, int gameModeId, long timePlay, Dictionary<string, object> gainedRewards = null, Dictionary<string, object> spentResources = null, long? timestamp = null)
        {
            this.Level          = level;
            this.Status         = status;
            this.GameModeId     = gameModeId;
            this.TimePlay       = timePlay;
            this.GainedRewards  = gainedRewards;
            this.spentResources = spentResources;
            this.Timestamp      = timestamp;
        }
    }
}