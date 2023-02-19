namespace TheOneStudio.UITemplate.UITemplate.Services
{
    using System;
    using Cysharp.Threading.Tasks;
    using Newtonsoft.Json;
    using UnityEngine;
    using UnityEngine.Networking;

    public interface IInternetService
    {
        UniTask<DateTime> GetCurrentTimeAsync();
        bool              CheckInternet(string url);
    }

    public class InternetService : IInternetService
    {
        public static string WorldTimeAPIUrl => "http://worldtimeapi.org/api";

        public async UniTask<DateTime> GetCurrentTimeAsync()
        {
            try
            {
                return (await this.GetTimeIPAsync()).GetDateTime().GetValueOrDefault();
            }
            catch
            {
                Debug.LogError("No internet!");
            }

            return default;
        }

        public bool CheckInternet(string url) { return UnityWebRequest.Get(url).result == UnityWebRequest.Result.ConnectionError; }

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

            public DateTime? GetUtcDateTime()
            {
                if (DateTime.TryParse(this.UtcDatetime, out var time))
                {
                    return time;
                }

                return null;
            }
        }

        public async UniTask<WorldTimeAPIResponse> GetTimeIPAsync(string ip = null)
        {
            var url = $"{WorldTimeAPIUrl}/ip";

            if (ip is not null)
            {
                url += $"/{ip}";
            }

            using var webRequest = UnityWebRequest.Get(url);

            return JsonConvert.DeserializeObject<WorldTimeAPIResponse>((await webRequest.SendWebRequest()).downloadHandler.text);
        }
    }
}