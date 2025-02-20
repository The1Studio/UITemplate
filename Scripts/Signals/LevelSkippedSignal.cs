namespace TheOneStudio.UITemplate.UITemplate.Signals
{
    public class LevelSkippedSignal
    {
        public int    Level;
        public string Mode;
        public int    Time;
        
        public LevelSkippedSignal(int level, string mode, int time)
        {
            this.Level = level;
            this.Mode  = mode;
            this.Time  = time;
        }
    }
}