namespace TheOneStudio.UITemplate.UITemplate.Signals
{
    using System.Collections.Generic;

    public class LevelStartedSignal
    {
        public int                        Level;
        public string                     Mode;
        public float                      TimeStamp;
        public Dictionary<string, object> Metadata;

        public LevelStartedSignal(int level, string mode, float timeStamp, Dictionary<string, object> metadata = null)
        {
            this.Level     = level;
            this.Mode      = mode;
            this.TimeStamp = timeStamp;
            this.Metadata  = metadata;
        }
    }
}