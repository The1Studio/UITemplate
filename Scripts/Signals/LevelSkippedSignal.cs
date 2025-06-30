namespace TheOneStudio.UITemplate.UITemplate.Signals
{
    using System.Collections.Generic;

    public class LevelSkippedSignal
    {
        public int                        Level;
        public string                     Mode;
        public int                        Time;
        public Dictionary<string, object> Metadata;
        
        public LevelSkippedSignal(int level, string mode, int time, Dictionary<string, object> metadata = null)
        {
            this.Level    = level;
            this.Mode     = mode;
            this.Time     = time;
            this.Metadata = metadata;
        }
    }
}