namespace TheOneStudio.UITemplate.UITemplate.Signals
{
    using System.Collections.Generic;

    public class LevelEndedSignal
    {
        public int                        Level;
        public LevelEndStatus             EndStatus;
        public int                        Time;
        public int                        GameModeId;
        public Dictionary<string, int>    CurrentIdToValue;
        public Dictionary<string, object> SpentResources;
        public long                       TimeBetweenLastSignal;
    }

    public enum LevelEndStatus
    {
        Completed,
        Failed,
        Skipped,
    }
}