namespace TheOneStudio.UITemplate.UITemplate.Leaderboard.Models
{
    using System;
    using System.Collections.Generic;
    using GameFoundation.Scripts.Utilities.Extension;
    using Newtonsoft.Json;
    using TheOneStudio.UITemplate.Models;

    public class UITemplateLeaderBoardData : UITemplateLocalData<UITemplateLeaderBoardDataController>
    {
        [JsonProperty] private readonly Dictionary<string, Record> records = new Dictionary<string, Record>();

        public Record this[object key] => this.records.GetOrAdd(key.ToString(), () => new Record());

        public class Record
        {
            public int AllTimeHighScore { get; set; }

            public Dictionary<DateTime, int> DailyHighScores { get; } = new Dictionary<DateTime, int>();

            public Dictionary<DateTime, int> WeeklyHighScores { get; } = new Dictionary<DateTime, int>();

            public Dictionary<DateTime, int> MonthlyHighScores { get; } = new Dictionary<DateTime, int>();

            public Dictionary<DateTime, int> YearlyHighScores { get; } = new Dictionary<DateTime, int>();
        }
    }
}