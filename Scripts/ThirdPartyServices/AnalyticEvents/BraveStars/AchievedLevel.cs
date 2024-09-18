namespace TheOneStudio.UITemplate.UITemplate.ThirdPartyServices.AnalyticEvents.BraveStars
{
    using Core.AnalyticServices.Data;

    public class AchievedLevel : IEvent
    {
        public int Level;

        public AchievedLevel(int level)
        {
            this.Level = level;
        }
    }
}