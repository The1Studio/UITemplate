namespace TheOneStudio.UITemplate.UITemplate.Signals
{
    using System.Collections.Generic;

    public class LevelEndedSignal
    {
        public int                        Level;
        public string                     Mode;
        public bool                       IsWin;
        public int                        Time;
        public Dictionary<string, int>    CurrentIdToValue;
        public Dictionary<string, object> Metadata;
        
        public LevelEndedSignal(int level, string mode, bool isWin, int time, Dictionary<string, int> currentIdToValue, Dictionary<string, object> metadata = null)
        {
            this.Level            = level;
            this.Mode             = mode;
            this.IsWin            = isWin;
            this.Time             = time;
            this.CurrentIdToValue = currentIdToValue;
            this.Metadata         = metadata;
        }
    }
}