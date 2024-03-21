namespace TheOneStudio.UITemplate.HighScore.Models
{
    using System;
    using System.Collections.Generic;
    using Newtonsoft.Json;
    using TheOneStudio.HyperCasual;
    using TheOneStudio.UITemplate.Models;

    public sealed class UITemplateHighScoreData : UITemplateLocalData<UITemplateHighScoreDataController>
    {
        [JsonProperty] private readonly Dictionary<string, TypeToTimes> keyToTypes = new();

        public TypeToTimes this[string key] => this.keyToTypes.GetOrAdd(key);

        public sealed class TypeToTimes
        {
            [JsonProperty] private readonly Dictionary<HighScoreType, TimeToHighScores> typeToTimes = new();

            public TimeToHighScores this[HighScoreType type] => this.typeToTimes.GetOrAdd(type);

            public sealed class TimeToHighScores
            {
                [JsonProperty] private readonly Dictionary<DateTime, List<int>> timeToHighScores = new();

                public List<int> this[DateTime time] => this.timeToHighScores.GetOrAdd(time);
            }
        }
    }
}