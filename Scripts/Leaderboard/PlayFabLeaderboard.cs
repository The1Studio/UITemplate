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

        private readonly UITemplateHighScoreDataController highScoreDataController;

        private readonly Dictionary<string, Dictionary<HighScoreType, PlayerLeaderboardEntry>>       keyToTypeToPlayerEntry = new();
        private readonly Dictionary<string, Dictionary<HighScoreType, List<PlayerLeaderboardEntry>>> keyToTypeToLeaderboard = new();

        public PlayFabLeaderboard(UITemplateHighScoreDataController highScoreDataController)
        {
            this.highScoreDataController = highScoreDataController;
        }

        #endregion

        private LoginResult     loginResult;
        private UserAccountInfo accountInfo;

        public string PlayerId    => this.loginResult.PlayFabId;
        public string DisplayName => this.accountInfo.TitleInfo?.DisplayName;

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

            await this.FetchAccountInfoAsync();
        }

        public async UniTask ChangeDisplayNameAsync(string name)
        {
            await InvokeAsync<UpdateUserTitleDisplayNameRequest, UpdateUserTitleDisplayNameResult>(
                PlayFabClientAPI.UpdateUserTitleDisplayName,
                new() { DisplayName = name }
            );
            await this.FetchAccountInfoAsync();
        }

        public async UniTask FetchLeaderboardAsync(string key = DEFAULT_KEY)
        {
            await SupportedTypes.Select(async type =>
            {
                await (this.FetchPlayerEntryAsync(key, type), this.FetchLeaderboardAsync(key, type));
            });
        }

        public PlayerLeaderboardEntry GetPlayerEntry(string key, HighScoreType type)
        {
            if (!this.keyToTypeToPlayerEntry.TryGetValue(key, out var typeToPlayerEntry)
                || !typeToPlayerEntry.TryGetValue(type, out var playerEntry)
            ) throw new InvalidOperationException("Please login & fetch leaderboard first");
            return playerEntry;
        }

        public IEnumerable<PlayerLeaderboardEntry> GetLeaderboard(string key, HighScoreType type)
        {
            if (!this.keyToTypeToLeaderboard.TryGetValue(key, out var typeToLeaderboard)
                || !typeToLeaderboard.TryGetValue(type, out var leaderboard)
            ) throw new InvalidOperationException("Please login & fetch leaderboard first");
            return leaderboard;
        }

        public async UniTask SubmitScoreAsync(string key = DEFAULT_KEY)
        {
            await SupportedTypes.Select(async type =>
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
            });
            await this.FetchLeaderboardAsync(key);
        }

        #region Default

        public PlayerLeaderboardEntry GetPlayerEntry(HighScoreType type) => this.GetPlayerEntry(DEFAULT_KEY, type);

        public IEnumerable<PlayerLeaderboardEntry> GetLeaderboard(HighScoreType type) => this.GetLeaderboard(DEFAULT_KEY, type);

        #endregion

        #region Private

        private static readonly HighScoreType[] SupportedTypes =
        {
            HighScoreType.Daily,
            HighScoreType.Weekly,
            HighScoreType.Monthly,
            HighScoreType.AllTime,
        };

        private async UniTask FetchAccountInfoAsync()
        {
            var result = await InvokeAsync<GetAccountInfoRequest, GetAccountInfoResult>(
                PlayFabClientAPI.GetAccountInfo,
                new() { PlayFabId = this.PlayerId }
            );
            this.accountInfo = result.AccountInfo;
        }

        private async UniTask FetchPlayerEntryAsync(string key, HighScoreType type)
        {
            var result = await InvokeAsync<GetLeaderboardAroundPlayerRequest, GetLeaderboardAroundPlayerResult>(
                PlayFabClientAPI.GetLeaderboardAroundPlayer,
                new() { StatisticName = $"{key}_{type}", MaxResultsCount = 1 }
            );
            var playerEntry = result.Leaderboard.FirstOrDefault(entry => entry.PlayFabId == this.PlayerId);
            this.keyToTypeToPlayerEntry.GetOrAdd(key)[type] = playerEntry;
        }

        private async UniTask FetchLeaderboardAsync(string key, HighScoreType type)
        {
            var result = await InvokeAsync<GetLeaderboardRequest, GetLeaderboardResult>(
                PlayFabClientAPI.GetLeaderboard,
                new() { StatisticName = $"{key}_{type}", MaxResultsCount = 100 }
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

        private delegate void PlayFabAction<in TRequest, out TResult>(TRequest request, Action<TResult> resultCallback, Action<PlayFabError> errorCallback, object customData = null, Dictionary<string, string> extraHeaders = null);

        #endregion
    }
}
#endif