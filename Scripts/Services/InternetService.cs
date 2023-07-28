namespace TheOneStudio.UITemplate.UITemplate.Services
{
    using System;
    using System.Net;
    using Cysharp.Threading.Tasks;
    using GameFoundation.Scripts.Utilities.LogService;
    using Newtonsoft.Json;
    using UnityEngine;
    using UnityEngine.Networking;
    using Zenject;

    public interface IInternetService
    {
        UniTask<DateTime> GetCurrentTimeAsync();
        UniTask<DateTime> GetCurrentUTCTimeAsync();
        bool              IsInternetAvailable { get; }
        UniTask<bool>     IsDifferentDay(DateTime timeCompare);
        bool              IsDifferentDay(DateTime timeCompare, DateTime newTime);
    }

    public class InternetService : IInternetService, IInitializable
    {
        private readonly ILogService logService;

        public static string WorldTimeAPIUrl  => "https://worldtimeapi.org/api";

        private bool isInternetAvailable = true;

        public InternetService(ILogService logService) { this.logService = logService; }

        public async UniTask<bool> IsDifferentDay(DateTime timeCompare)
        {
            var currentTime = await this.GetCurrentTimeAsync();

            return this.IsDifferentDay(timeCompare, currentTime);
        }

        public bool IsDifferentDay(DateTime timeCompare, DateTime newTime)
        {
            var day = (newTime - timeCompare).TotalDays;

            return day >= 1;
        }

        public int ToTalDiffDay(DateTime timeCompare, DateTime newTime)
        {
            var day = (newTime - timeCompare).TotalDays;

            return day >= 1 ? (int)day : 0;
        }

        public void Initialize() { this.CheckInternetInterval(); }

        private async UniTask CheckInternetInterval()
        {
            this.CheckInternet();
            await UniTask.Delay(TimeSpan.FromSeconds(2));
            _ = this.CheckInternetInterval();
        }

        private async UniTask<WorldTimeAPIResponse> GetTimeIPAsync()
        {
            var url     = $"{WorldTimeAPIUrl}/ip";
            var request = UnityWebRequest.Get(url);
            await request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.ConnectionError)
            {
                this.logService.Error("No internet!");
            }

            return JsonConvert.DeserializeObject<WorldTimeAPIResponse>(request.downloadHandler.text);
        }

        public async UniTask<DateTime> GetCurrentTimeAsync()
        {
            try
            {
                return (await this.GetTimeIPAsync()).GetDateTime().GetValueOrDefault();
            }
            catch
            {
                this.logService.Error("No internet!");
            }

            return default;
        }

        public async UniTask<DateTime> GetCurrentUTCTimeAsync()
        {
            try
            {
                return (await this.GetTimeIPAsync()).GetUTCDateTime().GetValueOrDefault();
            }
            catch
            {
                this.logService.Error("No internet!");
            }

            return default;
        }

        public bool IsInternetAvailable => this.isInternetAvailable;

        private void CheckInternet()
        {
            this.isInternetAvailable = Application.internetReachability != NetworkReachability.NotReachable;
        }

        public class WorldTimeAPIResponse
        {
            [JsonProperty("datetime")]     public string Datetime    { get; set; }
            [JsonProperty("utc_datetime")] public string UtcDatetime { get; set; }

            public DateTime? GetDateTime()
            {
                if (DateTime.TryParse(this.Datetime, out var time))
                {
                    return time;
                }

                return null;
            }

            public DateTime? GetUTCDateTime()
            {
                if (DateTime.TryParse(this.UtcDatetime, out var time))
                {
                    return time;
                }

                return null;
            }
        }
    }
}