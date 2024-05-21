namespace TheOneStudio.HyperCasual
{
    using System.Linq;
    using PlayFab.ClientModels;

    public static class PlayFabExtensions
    {
        public static string GetCountryCode(this PlayerProfileModel profile)
        {
            return profile.Locations?
                .Where(location => location.CountryCode is { })
                .LastOrDefault()?
                .CountryCode
                .ToString()
                .ToLower();
        }
    }
}