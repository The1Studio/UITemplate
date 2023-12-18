namespace TheOneStudio.UITemplate.UITemplate.Leaderboard
{
    using System;
    using System.Globalization;
    using GameFoundation.Scripts.Utilities.Extension;
    using TheOneStudio.UITemplate.UITemplate.Leaderboard.Models;
    using TheOneStudio.UITemplate.UITemplate.Leaderboard.Signals;
    using TheOneStudio.UITemplate.UITemplate.Models.Controllers;
    using Zenject;

    public class UITemplateLeaderBoardDataController : IUITemplateControllerData
    {
        private const string DEFAULT_KEY = "DEFAULT";

        #region Constructor

        private readonly UITemplateLeaderBoardData leaderBoardData;
        private readonly SignalBus                 signalBus;

        public UITemplateLeaderBoardDataController(UITemplateLeaderBoardData leaderBoardData, SignalBus signalBus)
        {
            this.leaderBoardData = leaderBoardData;
            this.signalBus       = signalBus;
        }

        #endregion

        public void SubmitScore(string key, DateTime dateTime, int score)
        {
            var allTimeHighScore = this.leaderBoardData[key].AllTimeHighScore;
            if (score > allTimeHighScore)
            {
                this.leaderBoardData[key].AllTimeHighScore = score;
                this.signalBus.Fire(new NewAllTimeHighScoreSignal(allTimeHighScore, score));
            }

            var day            = dateTime.Date;
            var dailyHighScore = this.leaderBoardData[key].DailyHighScores.GetOrDefault(day);
            if (score > dailyHighScore)
            {
                this.leaderBoardData[key].DailyHighScores[day] = score;
                this.signalBus.Fire(new NewDailyHighScoreSignal(dailyHighScore, score));
            }

            var week            = dateTime.GetFirstDayOfWeek();
            var weeklyHighScore = this.leaderBoardData[key].WeeklyHighScores.GetOrDefault(week);
            if (score > weeklyHighScore)
            {
                this.leaderBoardData[key].WeeklyHighScores[week] = score;
                this.signalBus.Fire(new NewWeeklyHighScoreSignal(weeklyHighScore, score));
            }

            var month            = dateTime.GetFirstDayOfMonth();
            var monthlyHighScore = this.leaderBoardData[key].MonthlyHighScores.GetOrDefault(month);
            if (score > monthlyHighScore)
            {
                this.leaderBoardData[key].MonthlyHighScores[month] = score;
                this.signalBus.Fire(new NewMonthlyHighScoreSignal(monthlyHighScore, score));
            }

            var year            = dateTime.GetFirstDayOfYear();
            var yearlyHighScore = this.leaderBoardData[key].YearlyHighScores.GetOrDefault(year);
            if (score > yearlyHighScore)
            {
                this.leaderBoardData[key].YearlyHighScores[year] = score;
                this.signalBus.Fire(new NewYearlyHighScoreSignal(yearlyHighScore, score));
            }
        }

        public int GetAllTimeHighScore(string key)
        {
            return this.leaderBoardData[key].AllTimeHighScore;
        }

        public int GetDailyHighScore(string key, DateTime dateTime)
        {
            return this.leaderBoardData[key].DailyHighScores.GetOrDefault(dateTime.Date);
        }

        public int GetWeeklyHighScore(string key, DateTime dateTime)
        {
            return this.leaderBoardData[key].WeeklyHighScores.GetOrDefault(dateTime.GetFirstDayOfWeek());
        }

        public int GetMonthlyHighScore(string key, DateTime dateTime)
        {
            return this.leaderBoardData[key].MonthlyHighScores.GetOrDefault(dateTime.GetFirstDayOfMonth());
        }

        public int GetYearlyHighScore(string key, DateTime dateTime)
        {
            return this.leaderBoardData[key].YearlyHighScores.GetOrDefault(dateTime.GetFirstDayOfYear());
        }

        #region Default Key

        public void SubmitScore(DateTime dateTime, int score) => this.SubmitScore(DEFAULT_KEY, dateTime, score);

        public int GetAllTimeHighScore() => this.GetAllTimeHighScore(DEFAULT_KEY);

        public int GetDailyHighScore(DateTime dateTime) => this.GetDailyHighScore(DEFAULT_KEY, dateTime);

        public int GetWeeklyHighScore(DateTime dateTime) => this.GetWeeklyHighScore(DEFAULT_KEY, dateTime);

        public int GetMonthlyHighScore(DateTime dateTime) => this.GetMonthlyHighScore(DEFAULT_KEY, dateTime);

        public int GetYearlyHighScore(DateTime dateTime) => this.GetYearlyHighScore(DEFAULT_KEY, dateTime);

        #endregion

        #region Default Time

        public void SubmitScore(string key, int score) => this.SubmitScore(key, DateTime.UtcNow, score);

        public int GetDailyHighScore(string key) => this.GetDailyHighScore(key, DateTime.UtcNow);

        public int GetWeeklyHighScore(string key) => this.GetWeeklyHighScore(key, DateTime.UtcNow);

        public int GetMonthlyHighScore(string key) => this.GetMonthlyHighScore(key, DateTime.UtcNow);

        public int GetYearlyHighScore(string key) => this.GetYearlyHighScore(key, DateTime.UtcNow);

        #endregion

        #region Default

        public void SubmitScore(int score) => this.SubmitScore(DEFAULT_KEY, DateTime.UtcNow, score);

        public int GetDailyHighScore() => this.GetDailyHighScore(DEFAULT_KEY, DateTime.UtcNow);

        public int GetWeeklyHighScore() => this.GetWeeklyHighScore(DEFAULT_KEY, DateTime.UtcNow);

        public int GetMonthlyHighScore() => this.GetMonthlyHighScore(DEFAULT_KEY, DateTime.UtcNow);

        public int GetYearlyHighScore() => this.GetYearlyHighScore(DEFAULT_KEY, DateTime.UtcNow);

        #endregion
    }

    public static class DateTimeExtensions
    {
        public static DateTime GetFirstDayOfWeek(this DateTime dateTime, DayOfWeek firstDayOfWeek)
        {
            var diff           = dateTime.DayOfWeek - firstDayOfWeek;
            if (diff < 0) diff += 7;
            return dateTime.AddDays(-1 * diff).Date;
        }

        public static DateTime GetFirstDayOfWeek(this DateTime dateTime, CultureInfo cultureInfo)
        {
            return GetFirstDayOfWeek(dateTime, cultureInfo.DateTimeFormat.FirstDayOfWeek);
        }

        public static DateTime GetFirstDayOfWeek(this DateTime dateTime)
        {
            return GetFirstDayOfWeek(dateTime, CultureInfo.InvariantCulture);
        }

        public static DateTime GetFirstDayOfMonth(this DateTime dateTime) => new DateTime(dateTime.Year, dateTime.Month, 1);

        public static DateTime GetFirstDayOfYear(this DateTime dateTime) => new DateTime(dateTime.Year, 1, 1);
    }
}