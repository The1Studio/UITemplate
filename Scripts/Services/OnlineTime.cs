namespace Utility
{
    using System;
    using System.Collections.Generic;
    using Cysharp.Threading.Tasks;
    using Newtonsoft.Json;
    using UnityEngine;
    using UnityEngine.Networking;

    public interface IOnlineTime
    {
        public string            Url { get; }
        public UniTask<DateTime> GetCurrentTimeAsync();
    }

    public class WorldTimeAPI : IOnlineTime
    {
        public string Url => "http://worldtimeapi.org/api";

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

        public class WorldTimeAPIResponse
        {
            [JsonProperty("abbreviation")] public string Abbreviation { get; set; }
            [JsonProperty("client_ip")]    public string ClientIp     { get; set; }
            [JsonProperty("datetime")]     public string Datetime     { get; set; }
            [JsonProperty("day_of_week")]  public int    DayOfWeek    { get; set; }
            [JsonProperty("day_of_year")]  public int    DayOfYear    { get; set; }
            [JsonProperty("dst")]          public bool   Dst          { get; set; }
            [JsonProperty("dst_from")]     public string DstFrom      { get; set; }
            [JsonProperty("dst_offset")]   public int    DstOffset    { get; set; }
            [JsonProperty("dst_until")]    public string DstUntil     { get; set; }
            [JsonProperty("raw_offset")]   public int    RawOffset    { get; set; }
            [JsonProperty("timezone")]     public string Timezone     { get; set; }
            [JsonProperty("unixtime")]     public long   Unixtime     { get; set; }
            [JsonProperty("utc_datetime")] public string UtcDatetime  { get; set; }
            [JsonProperty("utc_offset")]   public string UtcOffset    { get; set; }
            [JsonProperty("week_number")]  public int    WeekNumber   { get; set; }

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


        public async UniTask<List<string>> GetTimeZonesAsync(string area = null)
        {
            var url = $"{this.Url}/timezone";

            if (area is not null)
            {
                url += $"/{area}";
            }

            using var webRequest = UnityWebRequest.Get(url);

            return JsonConvert.DeserializeObject<List<string>>((await webRequest.SendWebRequest()).downloadHandler.text);
        }

        public async UniTask<WorldTimeAPIResponse> GetTimeAtAsync(string timezone, string region = null)
        {
            var url = $"{this.Url}/{timezone}";

            if (region is not null)
            {
                url += $"/{region}";
            }

            using var webRequest = UnityWebRequest.Get(url);

            return JsonConvert.DeserializeObject<WorldTimeAPIResponse>((await webRequest.SendWebRequest()).downloadHandler.text);
        }

        public async UniTask<WorldTimeAPIResponse> GetTimeIPAsync(string ip = null)
        {
            var url = $"{this.Url}/ip";

            if (ip is not null)
            {
                url += $"/{ip}";
            }

            using var webRequest = UnityWebRequest.Get(url);

            return JsonConvert.DeserializeObject<WorldTimeAPIResponse>((await webRequest.SendWebRequest()).downloadHandler.text);
        }
    }
}