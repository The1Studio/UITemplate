namespace TheOneStudio.UITemplate.HighScore
{
    using System;
    using GameFoundation.Scripts.Utilities.Extension;
    using TheOneStudio.UITemplate.HighScore.Models;
    using TheOneStudio.UITemplate.HighScore.Signals;
    using TheOneStudio.UITemplate.UITemplate.Models.Controllers;
    using Zenject;

    public class UITemplateHighScoreDataController : IUITemplateControllerData
    {
        public const string DEFAULT_KEY = "DEFAULT";

        #region Constructor

        private readonly UITemplateHighScoreData highScoreData;
        private readonly SignalBus               signalBus;

        public UITemplateHighScoreDataController(UITemplateHighScoreData highScoreData, SignalBus signalBus)
        {
            this.highScoreData = highScoreData;
            this.signalBus     = signalBus;
        }

        #endregion

        public void SubmitScore(string key, int newHighScore)
        {
            var allTimeHighScore = this.highScoreData[key].AllTimeHighScore;
            if (newHighScore > allTimeHighScore)
            {
                this.highScoreData[key].AllTimeHighScore = newHighScore;
                this.signalBus.Fire(new NewAllTimeHighScoreSignal(allTimeHighScore, newHighScore));
            }

            var day            = DateTime.UtcNow.Date;
            var dailyHighScore = this.highScoreData[key].DailyHighScores.GetOrDefault(day);
            if (newHighScore > dailyHighScore)
            {
                this.highScoreData[key].DailyHighScores[day] = newHighScore;
                this.signalBus.Fire(new NewDailyHighScoreSignal(dailyHighScore, newHighScore));
            }

            var week            = day.GetFirstDayOfWeek();
            var weeklyHighScore = this.highScoreData[key].WeeklyHighScores.GetOrDefault(week);
            if (newHighScore > weeklyHighScore)
            {
                this.highScoreData[key].WeeklyHighScores[week] = newHighScore;
                this.signalBus.Fire(new NewWeeklyHighScoreSignal(weeklyHighScore, newHighScore));
            }

            var month            = day.GetFirstDayOfMonth();
            var monthlyHighScore = this.highScoreData[key].MonthlyHighScores.GetOrDefault(month);
            if (newHighScore > monthlyHighScore)
            {
                this.highScoreData[key].MonthlyHighScores[month] = newHighScore;
                this.signalBus.Fire(new NewMonthlyHighScoreSignal(monthlyHighScore, newHighScore));
            }

            var year            = day.GetFirstDayOfYear();
            var yearlyHighScore = this.highScoreData[key].YearlyHighScores.GetOrDefault(year);
            if (newHighScore > yearlyHighScore)
            {
                this.highScoreData[key].YearlyHighScores[year] = newHighScore;
                this.signalBus.Fire(new NewYearlyHighScoreSignal(yearlyHighScore, newHighScore));
            }
        }

        public int GetAllTimeHighScore(string key)
        {
            return this.highScoreData[key].AllTimeHighScore;
        }

        public int GetDailyHighScore(string key)
        {
            return this.highScoreData[key].DailyHighScores.GetOrDefault(DateTime.UtcNow.Date);
        }

        public int GetWeeklyHighScore(string key)
        {
            return this.highScoreData[key].WeeklyHighScores.GetOrDefault(DateTime.UtcNow.GetFirstDayOfWeek());
        }

        public int GetMonthlyHighScore(string key)
        {
            return this.highScoreData[key].MonthlyHighScores.GetOrDefault(DateTime.UtcNow.GetFirstDayOfMonth());
        }

        public int GetYearlyHighScore(string key)
        {
            return this.highScoreData[key].YearlyHighScores.GetOrDefault(DateTime.UtcNow.GetFirstDayOfYear());
        }

        #region Default

        public void SubmitScore(int newHighScore) => this.SubmitScore(DEFAULT_KEY, newHighScore);

        public int GetAllTimeHighScore() => this.GetAllTimeHighScore(DEFAULT_KEY);

        public int GetDailyHighScore() => this.GetDailyHighScore(DEFAULT_KEY);

        public int GetWeeklyHighScore() => this.GetWeeklyHighScore(DEFAULT_KEY);

        public int GetMonthlyHighScore() => this.GetMonthlyHighScore(DEFAULT_KEY);

        public int GetYearlyHighScore() => this.GetYearlyHighScore(DEFAULT_KEY);

        #endregion
    }
}