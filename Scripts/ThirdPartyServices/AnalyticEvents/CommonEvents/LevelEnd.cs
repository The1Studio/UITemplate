namespace TheOneStudio.UITemplate.UITemplate.ThirdPartyServices.AnalyticEvents.CommonEvents
{
    using Core.AnalyticServices.Data;

    public class LevelEnd : IEvent
    {
        public int    level;
        public string status;
        public int    worldId;
        public double timePlay;
        public long   timerStamp;

        public LevelEnd(int level, string status, int worldId, double timePlay, long timeStamp)
        {
            this.level      = level;
            this.status     = status;
            this.worldId    = worldId;
            this.timePlay   = timePlay;
            this.timerStamp = timeStamp;
        }
    }
}