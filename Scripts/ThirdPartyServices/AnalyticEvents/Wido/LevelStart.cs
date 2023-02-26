namespace TheOneStudio.UITemplate.UITemplate.ThirdPartyServices.AnalyticEvents.Wido
{
    using Core.AnalyticServices.Data;

    public class LevelStart : IEvent
    {
        public int level;
        public bool restart;
        
        public LevelStart(int level, bool restart)
        {
            this.level   = level;
            this.restart = restart;
        }
    }
}