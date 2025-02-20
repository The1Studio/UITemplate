namespace TheOneStudio.UITemplate.UITemplate.Signals
{
    public class LevelStartedSignal
    {
        public int    Level;
        public string Mode;

        public LevelStartedSignal(int level, string mode)
        {
            this.Level = level;
            this.Mode  = mode;
        }
    }
}