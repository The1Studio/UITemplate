namespace TheOneStudio.UITemplate.UITemplate.ThirdPartyServices.AnalyticEvents.Mirai
{
    using Core.AnalyticServices.Data;

    public class LevelEnd : IEvent
    {
        public int    level;
        public string status;
        public int    worldId;
        public float  timePlay;
        public float  timerStamp;

        public LevelEnd(int level,string status, int worldId,float timePlay, float timeStamp)
        {
            this.level      = level;
            this.status     = status;
            this.worldId    = worldId;
            this.timePlay   = timePlay;
            this.timerStamp = timeStamp;
        }
    }
}