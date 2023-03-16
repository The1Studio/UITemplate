namespace TheOneStudio.UITemplate.UITemplate.ThirdPartyServices.AnalyticEvents.ABI.Firebase
{
    using Core.AnalyticServices.Data;

    public class LevelStart: IEvent
    {
        public int Level;
        public int Gold;
        
        public LevelStart(int level, int gold)
        {
            this.Level = level;
            this.Gold  = gold;
        } 
    }
}