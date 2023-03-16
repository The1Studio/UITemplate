namespace TheOneStudio.UITemplate.UITemplate.ThirdPartyServices.AnalyticEvents.ABI
{
    using Core.AnalyticServices.Data;

    public class LevelFail: IEvent
    {
        public int Level;
        public int TimeCount;

        public LevelFail(int level, int timeCount)
        {
            this.Level     = level;
            this.TimeCount = timeCount;
        }
    }
}