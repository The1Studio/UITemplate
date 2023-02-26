namespace TheOneStudio.UITemplate.UITemplate.ThirdPartyServices.AnalyticEvents
{
    using Core.AnalyticServices.Data;

    public interface IAnalyticEventFactory
    {
        IEvent InterstitialShow(int level, string place);
        IEvent RewardedVideoShow(int level, string place);
        IEvent LevelLose(int level, int time);
        IEvent LevelStart(int level);
        IEvent LevelWin(int level, int time);
        IEvent LevelSkipped(int level, int time);
        void ForceUpdateAllProperties();
    }
}