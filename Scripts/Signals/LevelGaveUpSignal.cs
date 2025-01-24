namespace TheOneStudio.UITemplate.UITemplate.Signals
{
    using System.Collections.Generic;

    public class LevelGaveUpSignal
    {
        public int                        Level          { get; }
        public Dictionary<string, object> AdditionalData { get; }

        public LevelGaveUpSignal(int level, Dictionary<string, object> additionalData = null)
        {
            this.Level          = level;
            this.AdditionalData = additionalData;
        }
    }
}