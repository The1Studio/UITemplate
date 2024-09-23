namespace TheOneStudio.UITemplate.UITemplate.Signals
{
    using System.Collections.Generic;

    public class LevelEndedSignal
    {
        public int                     Level;
        public bool                    IsWin;
        public double                  Time;
        public Dictionary<string, int> CurrentIdToValue;
    }
}