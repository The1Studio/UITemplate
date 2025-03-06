namespace TheOneStudio.UITemplate.UITemplate.Signals
{
    using System.Collections.Generic;

    public class LevelEndedSignal
    {
        public int                     Level;
        public bool                    IsWin;
        public int                     Time;
        public Dictionary<string, int> CurrentIdToValue;
    }
}