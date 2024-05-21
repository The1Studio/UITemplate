#if THEONE_PLAYFAB
namespace TheOneStudio.HyperCasual
{
    using System.Collections.Generic;
    using PlayFab.ClientModels;
    using TheOneStudio.UITemplate.HighScore.Models;

    public class PlayFabConfig
    {
        public static PlayFabConfig Default => new();

        public List<HighScoreType> UsedHighScoreTypesTypes { get; set; } = new()
        {
            HighScoreType.Daily,
            HighScoreType.Weekly,
            HighScoreType.Monthly,
            HighScoreType.AllTime,
        };

        public PlayerProfileViewConstraints ProfileConstraints { get; set; } = new()
        {
            ShowDisplayName = true,
            ShowAvatarUrl   = true,
            ShowLocations   = true,
        };
    }
}
#endif