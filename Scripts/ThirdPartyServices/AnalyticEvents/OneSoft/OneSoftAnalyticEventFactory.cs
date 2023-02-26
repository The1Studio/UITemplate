namespace TheOneStudio.UITemplate.UITemplate.ThirdPartyServices.AnalyticEvents.OneSoft
{
    using Core.AnalyticServices.Data;

    public class OneSoftAnalyticEventFactory : IAnalyticEventFactory
    {
        public IEvent InterstitialShow(int  level, string place) => new InterstitialShow(level, place);
        public IEvent RewardedVideoShow(int level, string place) => new RewardedVideoShow(level, place);
        public IEvent LevelLose(int         level, int    time)  => new LevelLose(level, time);
        public IEvent LevelStart(int        level)           => new LevelStart(level);
        public IEvent LevelWin(int          level, int time) => new LevelWin(level, time);
        public IEvent LevelSkipped(int      level, int time) => new LevelSkipped(level, time);
        public void ForceUpdateAllProperties()
        {
        }
    }
}