namespace TheOneStudio.UITemplate.UITemplate.Signals
{
    public class LevelStartedSignal
    {
        public int    Level;
        public string Mode;
        public float  TimeStamp;

        public LevelStartedSignal(int level, string mode, float timeStamp)
        {
            this.Level     = level;
            this.Mode      = mode;
            this.TimeStamp = timeStamp;
        }
    }
}