﻿#if THEONE_PLAYFAB
namespace TheOneStudio.HyperCasual
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Cysharp.Threading.Tasks;
    using PlayFab;
    using PlayFab.ClientModels;
    using TheOneStudio.UITemplate.HighScore;
    using TheOneStudio.UITemplate.HighScore.Models;
    using UnityEngine.Device;

    public sealed class PlayFabLeaderboard
    {
        private const string DEFAULT_KEY = UITemplateHighScoreDataController.DEFAULT_KEY;

        #region Constructor

        private readonly PlayFabConfig                     config;
        private readonly UITemplateHighScoreDataController highScoreDataController;

        private readonly Dictionary<string, Dictionary<HighScoreType, PlayerLeaderboardEntry>>       keyToTypeToPlayerEntry = new();
        private readonly Dictionary<string, Dictionary<HighScoreType, List<PlayerLeaderboardEntry>>> keyToTypeToLeaderboard = new();

        public PlayFabLeaderboard(PlayFabConfig config, UITemplateHighScoreDataController highScoreDataController)
        {
            this.config                  = config;
            this.highScoreDataController = highScoreDataController;
        }

        #endregion

        private LoginResult        loginResult;
        private PlayerProfileModel profile;

        public bool   IsLoggedIn  => this.loginResult is { };
        public string PlayerId    => this.loginResult?.PlayFabId;
        public string DisplayName => this.profile?.DisplayName;
        public string Avatar      => this.profile?.AvatarUrl;
        public string CountryCode => this.profile?.GetCountryCode();

        public async UniTask LoginAsync()
        {
            #if UNITY_EDITOR
            this.loginResult = await InvokeAsync<LoginWithCustomIDRequest, LoginResult>(
                PlayFabClientAPI.LoginWithCustomID,
                new() { CustomId = SystemInfo.deviceUniqueIdentifier, CreateAccount = true }
            );
            #elif UNITY_ANDROID
            this.loginResult = await InvokeAsync<LoginWithAndroidDeviceIDRequest, LoginResult>(
                PlayFabClientAPI.LoginWithAndroidDeviceID,
                new() { AndroidDeviceId = SystemInfo.deviceUniqueIdentifier, CreateAccount = true }
            );
            #elif UNITY_IOS
            this.loginResult = await InvokeAsync<LoginWithIOSDeviceIDRequest, LoginResult>(
                PlayFabClientAPI.LoginWithIOSDeviceID,
                new() { DeviceId = SystemInfo.deviceUniqueIdentifier, CreateAccount = true }
            );
            #endif

            await this.FetchProfileAsync();
        }

        public async UniTask ChangeDisplayNameAsync(string name)
        {
            await InvokeAsync<UpdateUserTitleDisplayNameRequest, UpdateUserTitleDisplayNameResult>(
                PlayFabClientAPI.UpdateUserTitleDisplayName,
                new() { DisplayName = name }
            );
            await this.FetchProfileAsync();
        }

        public async UniTask ChangeAvatarAsync(string url)
        {
            await InvokeAsync<UpdateAvatarUrlRequest, EmptyResponse>(
                PlayFabClientAPI.UpdateAvatarUrl,
                new() { ImageUrl = url }
            );
            await this.FetchProfileAsync();
        }

        public async UniTask UpdateUserDataAsync(Dictionary<string, string> data, UserDataPermission? permission = null, List<string> keysToRemove = null)
        {
            await InvokeAsync<UpdateUserDataRequest, UpdateUserDataResult>(
                PlayFabClientAPI.UpdateUserData,
                new()
                {
                    Data         = data,
                    Permission   = permission,
                    KeysToRemove = keysToRemove,
                }
            );
        }

        public async UniTask<Dictionary<string, UserDataRecord>> GetUserDataAsync(List<string> keys, string playFabId = null)
        {
            var result = await InvokeAsync<GetUserDataRequest, GetUserDataResult>(
                PlayFabClientAPI.GetUserData,
                new()
                {
                    Keys      = keys,
                    PlayFabId = playFabId,
                }
            );
            return result.Data;
        }

        public bool IsLeaderboardFetched(string key = DEFAULT_KEY)
        {
            return this.keyToTypeToPlayerEntry.TryGetValue(key, out var typeToPlayerEntry)
                && typeToPlayerEntry.Count == this.config.UsedHighScoreTypesTypes.Count
                && this.keyToTypeToLeaderboard.TryGetValue(key, out var typeToLeaderboard)
                && typeToLeaderboard.Count == this.config.UsedHighScoreTypesTypes.Count;
        }

        public async UniTask FetchLeaderboardAsync(string key = DEFAULT_KEY)
        {
            await this.config.UsedHighScoreTypesTypes.Select(async type =>
            {
                await (this.FetchPlayerEntryAsync(key, type), this.FetchLeaderboardAsync(key, type));
            });
        }

        public PlayerLeaderboardEntry GetPlayerEntry(string key, HighScoreType type)
        {
            this.ThrowIfNotSupported(type);
            if (!this.keyToTypeToPlayerEntry.TryGetValue(key, out var typeToPlayerEntry)
                || !typeToPlayerEntry.TryGetValue(type, out var playerEntry)
            ) throw new InvalidOperationException("Please login & fetch leaderboard first");
            return playerEntry;
        }

        public IEnumerable<PlayerLeaderboardEntry> GetLeaderboard(string key, HighScoreType type)
        {
            this.ThrowIfNotSupported(type);
            if (!this.keyToTypeToLeaderboard.TryGetValue(key, out var typeToLeaderboard)
                || !typeToLeaderboard.TryGetValue(type, out var leaderboard)
            ) throw new InvalidOperationException("Please login & fetch leaderboard first");
            return leaderboard;
        }

        public async UniTask SubmitScoreAsync(string key = DEFAULT_KEY)
        {
            foreach (var type in this.config.UsedHighScoreTypesTypes)
            {
                var statisticName = $"{key}_{type}";
                var statistics = await InvokeAsync<GetPlayerStatisticsRequest, GetPlayerStatisticsResult>(
                    PlayFabClientAPI.GetPlayerStatistics,
                    new() { StatisticNames = new() { statisticName } }
                );
                var statistic    = statistics.Statistics.FirstOrDefault();
                var oldHighScore = statistic?.Value ?? 0;
                var newHighScore = this.highScoreDataController.GetHighScore(key, type);
                if (newHighScore <= oldHighScore) return;
                var version = statistic?.Version;
                await InvokeAsync<UpdatePlayerStatisticsRequest, UpdatePlayerStatisticsResult>(
                    PlayFabClientAPI.UpdatePlayerStatistics,
                    new() { Statistics = new() { new() { StatisticName = statisticName, Value = newHighScore, Version = version } } }
                );
            }
            await this.FetchLeaderboardAsync(key);
        }

        #region Default

        public PlayerLeaderboardEntry GetPlayerEntry(HighScoreType type) => this.GetPlayerEntry(DEFAULT_KEY, type);

        public IEnumerable<PlayerLeaderboardEntry> GetLeaderboard(HighScoreType type) => this.GetLeaderboard(DEFAULT_KEY, type);

        #endregion

        #region Private

        private async UniTask FetchProfileAsync()
        {
            var result = await InvokeAsync<GetPlayerProfileRequest, GetPlayerProfileResult>(
                PlayFabClientAPI.GetPlayerProfile,
                new()
                {
                    PlayFabId          = this.PlayerId,
                    ProfileConstraints = this.config.ProfileConstraints,
                }
            );
            this.profile = result.PlayerProfile;
        }

        private async UniTask FetchPlayerEntryAsync(string key, HighScoreType type)
        {
            var result = await InvokeAsync<GetLeaderboardAroundPlayerRequest, GetLeaderboardAroundPlayerResult>(
                PlayFabClientAPI.GetLeaderboardAroundPlayer,
                new()
                {
                    StatisticName      = $"{key}_{type}",
                    MaxResultsCount    = 1,
                    ProfileConstraints = this.config.ProfileConstraints,
                }
            );
            var playerEntry = result.Leaderboard.FirstOrDefault(entry => entry.PlayFabId == this.PlayerId);
            this.keyToTypeToPlayerEntry.GetOrAdd(key)[type] = playerEntry;
            if (playerEntry is { })
            {
                var highScore = this.highScoreDataController.GetHighScore(key, type);
                if (playerEntry.StatValue > highScore)
                {
                    this.highScoreDataController.SubmitScore(key, type, playerEntry.StatValue);
                }
            }
        }

        private async UniTask FetchLeaderboardAsync(string key, HighScoreType type)
        {
            var result = await InvokeAsync<GetLeaderboardRequest, GetLeaderboardResult>(
                PlayFabClientAPI.GetLeaderboard,
                new()
                {
                    StatisticName      = $"{key}_{type}",
                    MaxResultsCount    = 100,
                    ProfileConstraints = this.config.ProfileConstraints,
                }
            );
            var leaderboard = result.Leaderboard;
            this.keyToTypeToLeaderboard.GetOrAdd(key)[type] = leaderboard;
        }

        private static async UniTask<TResult> InvokeAsync<TRequest, TResult>(PlayFabAction<TRequest, TResult> action, TRequest request)
        {
            var tcs = new UniTaskCompletionSource<TResult>();
            action(
                request,
                result => tcs.TrySetResult(result),
                error => tcs.TrySetException(new(error.ErrorMessage))
            );
            return await tcs.Task;
        }

        private void ThrowIfNotSupported(HighScoreType type)
        {
            if (!this.config.UsedHighScoreTypesTypes.Contains(type)) throw new NotSupportedException($"{type} high score is not supported. Please change config");
        }

        private delegate void PlayFabAction<in TRequest, out TResult>(TRequest request, Action<TResult> resultCallback, Action<PlayFabError> errorCallback, object customData = null, Dictionary<string, string> extraHeaders = null);

        #endregion
    }
}
#endif