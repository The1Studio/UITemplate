namespace TheOneStudio.UITemplate.UITemplate.Services.Segments.DifficultySegment.Data
{
    public class PlayerLevelProgress
    {
        public int   LevelID;
        public int   Attempts;
        public float TotalTime;
        public int   Failures;
        public int   BoostersUsed;
        public float LastBoosterUsedAt; // from 0.0 to 1.0
        public bool  Completed;
    }
}