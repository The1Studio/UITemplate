namespace TheOneStudio.UITemplate.HighScore.Models
{
    using System;
    using System.Collections.Generic;
    using GameFoundation.Scripts.Utilities.Extension;
    using Newtonsoft.Json;
    using TheOneStudio.UITemplate.Models;

    public class UITemplateHighScoreData : UITemplateLocalData<UITemplateHighScoreDataController>
    {
        [JsonProperty] private readonly Dictionary<string, Record> records = new();

        public Record this[string key] => this.records.GetOrAdd(key, () => new Record());

        public class Record
        {
            public int AllTimeHighScore { get; set; }

            public Dictionary<DateTime, int> DailyHighScores { get; } = new();

            public Dictionary<DateTime, int> WeeklyHighScores { get; } = new();

            public Dictionary<DateTime, int> MonthlyHighScores { get; } = new();

            public Dictionary<DateTime, int> YearlyHighScores { get; } = new();
        }
    }
}