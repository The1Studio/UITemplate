namespace TheOneStudio.UITemplate.ThirdPartyServices.AnalyticEvents.Level
{
    using System.Collections.Generic;
    using Core.AnalyticServices.Data;

    public class LevelEnd : IEvent
    {
        public int                        Level          { get; set; }
        public LevelEnd.Status            EndStatus      { get; set; }
        public int                        GameModeId     { get; set; }
        public long                       TimePlay       { get; set; }
        public Dictionary<string, object> GainedRewards  { get; set; }
        public Dictionary<string, object> SpendResources { get; set; }
        public long                       Timestamp      { get; set; }

        public LevelEnd(int level, LevelEnd.Status endStatus, int gameModeId, long timePlay, Dictionary<string, object> gainedRewards, Dictionary<string, object> spendResources, long timestamp)
        {
            this.Level          = level;
            this.EndStatus      = endStatus;
            this.GameModeId     = gameModeId;
            this.TimePlay       = timePlay;
            this.GainedRewards  = gainedRewards;
            this.SpendResources = spendResources;
            this.Timestamp      = timestamp;
        }

        public enum Status
        {
            Complete,
            Fail,
            Skip,
            Quit
        }
    }
}