namespace TheOneStudio.UITemplate.UITemplate.ThirdPartyServices.AnalyticEvents.ABI
{
    using Core.AnalyticServices.Data;

    public class LevelStart : IEvent
    {
        public int Level;
        public int Gold;
        public int Session;

        public LevelStart(int level, int gold, int session)
        {
            this.Level   = level;
            this.Gold    = gold;
            this.Session = session;
        }
    }
}