namespace TheOneStudio.UITemplate.UITemplate.Services
{
    using System;
    using Cysharp.Threading.Tasks;
    using GameFoundation.Scripts.Utilities.LogService;
    using Newtonsoft.Json;
    using UnityEngine.Networking;
    using Zenject;

    public interface IInternetService
    {
        UniTask<DateTime> GetCurrentTimeAsync();
        UniTask<DateTime> GetCurrentUTCTimeAsync();
        bool              IsInternetAvailable { get; }
    }

    public class InternetService : IInternetService, IInitializable
    {
        private readonly ILogService logService;

        public static string WorldTimeAPIUrl  => "http://worldtimeapi.org/api";
        public static string CheckInternetUrl => "https://www.google.com/";


        private bool cachedInternetStatus;

        public InternetService(ILogService logService) { this.logService = logService; }


        public void Initialize() { this.CheckInternetInterval(); }

        private async void CheckInternetInterval()
        {
            this.CheckInternet();
            await UniTask.Delay(TimeSpan.FromSeconds(2));
            this.CheckInternetInterval();
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

        public bool IsInternetAvailable => this.cachedInternetStatus;

        private void CheckInternet() { this.cachedInternetStatus = UnityWebRequest.Get(CheckInternetUrl).result == UnityWebRequest.Result.ConnectionError; }


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