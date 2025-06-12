namespace TheOneStudio.UITemplate.UITemplate.Services.Segments.DifficultySegment.Data
{
    using System.Collections.Generic;

    public class PlayerProfile
    {
        public string                    PlayerID;
        public List<PlayerLevelProgress> LevelHistory = new();
        public UserSegmentType           SegmentType  = UserSegmentType.Undefined;
    }
}