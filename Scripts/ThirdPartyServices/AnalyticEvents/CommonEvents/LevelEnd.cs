namespace TheOneStudio.UITemplate.UITemplate.ThirdPartyServices.AnalyticEvents.CommonEvents
{
    using System.Collections.Generic;
    using Core.AnalyticServices.Data;

    /// <summary>
    /// Event for when a level ends.
    /// </summary>
    public class LevelEnd : IEvent
    {
        /// <summary> Ended level.</summary>
        public int Level { get; set; }

        /// <summary> Level status: Completed, Failed, Skipped or Quit.</summary>
        public string Status { get; set; }

        /// <summary> Game mode id.</summary>
        public int GameModeId { get; set; }

        /// <summary> Time played in milliseconds.</summary>
        public long TimePlay { get; set; }

        /// <summary> Timestamp of the event.</summary>
        public long TimeBetweenLastEvent { get; set; }

        /// <summary> Resource spent while playing level.</summary>
        public Dictionary<string, object> SpentResources { get; set; }

        public LevelEnd(int level, string status, int gameModeId, long timePlay, long timeBetweenLastEvent, Dictionary<string, object> spentResources = null)
        {
            this.Level                = level;
            this.Status               = status;
            this.GameModeId           = gameModeId;
            this.TimePlay             = timePlay;
            this.TimeBetweenLastEvent = timeBetweenLastEvent;
            this.SpentResources       = spentResources;
        }
    }
}