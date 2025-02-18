using Core.AnalyticServices.Data;

namespace TheOneStudio.UITemplate.UITemplate.ThirdPartyServices.AnalyticEvents.TheOne
{
    public class LevelEnd : IEvent
    {
        public int    level;
        public bool success;
        public string   playMode;
        public string   reason;
        public int    worldId;
        public double timePlay;
        public double adDuration;
        public long   timerStamp;

        public LevelEnd(int level, bool success, string playMode, string reason, int worldId, double timePlay, double adDuration, long timerStamp)
        {
            this.level      = level;
            this.success    = success;
            this.playMode   = playMode;
            this.reason     = reason;
            this.worldId    = worldId;
            this.timePlay   = timePlay;
            this.adDuration = adDuration;
            this.timerStamp = timerStamp;
        }
    }
}