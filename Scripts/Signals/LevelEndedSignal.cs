namespace TheOneStudio.UITemplate.UITemplate.Signals
{
    using System.Collections.Generic;
    using BlueprintFlow.Signals;

    public class LevelEndedSignal:ISignal
    {
        public int                     Level;
        public bool                    IsWin;
        public int                     Time;
        public Dictionary<string, int> CurrentIdToValue;
    }
}