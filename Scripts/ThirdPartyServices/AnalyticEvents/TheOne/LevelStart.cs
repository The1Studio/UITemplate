using Core.AnalyticServices.Data;

namespace TheOneStudio.UITemplate.UITemplate.ThirdPartyServices.AnalyticEvents.TheOne
{
    public class LevelStart : IEvent
    {
        public int    level;
        public int    gold;
        public int    worldId;
        public string playMode;
        public long   timeStamp;

        public LevelStart(int level, int gold,string playMode = "classic", int worldId = 0, long? timeStamp = null)
        {
            this.level    = level;
            this.gold     = gold;
            this.worldId  = worldId;
            this.playMode = playMode;
            if (timeStamp != null) this.timeStamp = (long)timeStamp;
        }
    }
}