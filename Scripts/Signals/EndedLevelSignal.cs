namespace UITemplate.Scripts.Signals
{
    using System.Collections.Generic;

    public class EndedLevelSignal
    {
        public Type                    EndType;
        public Dictionary<string, int> CurrentIdToValue;

        public enum Type
        {
            Succeed,
            Failed,
            Skipped
        }
    }
}