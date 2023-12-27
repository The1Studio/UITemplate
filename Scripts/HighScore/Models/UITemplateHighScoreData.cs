namespace TheOneStudio.UITemplate.HighScore.Models
{
    using System;
    using System.Collections.Generic;
    using GameFoundation.Scripts.Utilities.Extension;
    using Newtonsoft.Json;
    using TheOneStudio.UITemplate.Models;

    public sealed class UITemplateHighScoreData : UITemplateLocalData<UITemplateHighScoreDataController>
    {
        [JsonProperty] private readonly Dictionary<string, TypeToTimes> keyToTypes = new();

        public TypeToTimes this[string key] => this.keyToTypes.GetOrAdd(key, () => new TypeToTimes());

        public sealed class TypeToTimes
        {
            [JsonProperty] private readonly Dictionary<HighScoreType, TimeToHighScore> typeToTimes = new();

            public TimeToHighScore this[HighScoreType type] => this.typeToTimes.GetOrAdd(type, () => new TimeToHighScore());

            public sealed class TimeToHighScore
            {
                [JsonProperty] private readonly Dictionary<DateTime, int> timeToHighScore = new();

                public int this[DateTime time]
                {
                    get => this.timeToHighScore.GetOrDefault(time);
                    set => this.timeToHighScore[time] = value;
                }
            }
        }
    }
}