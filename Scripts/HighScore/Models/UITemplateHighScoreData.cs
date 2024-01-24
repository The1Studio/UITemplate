namespace TheOneStudio.UITemplate.HighScore.Models
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using GameFoundation.Scripts.Utilities.Extension;
    using Newtonsoft.Json;
    using TheOneStudio.UITemplate.Models;

    public sealed class UITemplateHighScoreData : UITemplateLocalData<UITemplateHighScoreDataController>
    {
        [JsonProperty] private readonly Dictionary<string, TypeToTimes> keyToTypes = new();

        public TypeToTimes this[string key] => this.keyToTypes.GetOrAdd(key, () => new TypeToTimes());

        public sealed class TypeToTimes
        {
            [JsonProperty] private readonly Dictionary<HighScoreType, TimeToHighScore> typeToTimes          = new();
            [JsonProperty] private readonly Dictionary<HighScoreType, AllHighScore>    lastTimeToHighScores = new();

            public TimeToHighScore this[HighScoreType type] => this.typeToTimes.GetOrAdd(type, () => new TimeToHighScore());

            public void AddAllHighScore(HighScoreType type, DateTime time, int score)
            {
                var allHighScore = this.lastTimeToHighScores.GetOrAdd(type, () => new AllHighScore());
                allHighScore.SetAllHighScore(time,score);
            }
            public AllHighScore GetAllHighScore(HighScoreType type)
            {
                var allHighScore = this.lastTimeToHighScores.GetOrAdd(type, () => new AllHighScore());
                return allHighScore;
            }

            public sealed class TimeToHighScore
            {
                [JsonProperty] private readonly Dictionary<DateTime, int> timeToHighScore = new();

                public int this[DateTime time]
                {
                    get => this.timeToHighScore.GetOrDefault(time);
                    set => this.timeToHighScore[time] = value;
                }
            }
            public sealed class AllHighScore
            {
                [JsonProperty] public readonly Dictionary<DateTime,int> topUserHighScores = new();
                public void SetAllHighScore(DateTime time, int highScore)
                {
                    DateTime currentSecond = new DateTime(time.Year, time.Month, time.Day, time.Hour, time.Minute, time.Second);

                    this.topUserHighScores.Add(currentSecond, highScore);
                    var sortedHighScores = this.topUserHighScores.OrderByDescending(x => x.Value);
                    var topHighScores   = sortedHighScores.Take(3);
                    var newHighScores = new Dictionary<DateTime, int>(topHighScores);
                    this.topUserHighScores.Clear();
                    foreach (var entry in newHighScores)
                    {
                        this.topUserHighScores.Add(entry.Key, entry.Value);
                    }
                }
            }
        }
    }
}