namespace TheOneStudio.UITemplate.UITemplate.Leaderboard
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using GameFoundation.Scripts.Utilities.Extension;
    using TheOneStudio.UITemplate.UITemplate.Leaderboard.Models;
    using TheOneStudio.UITemplate.UITemplate.Leaderboard.Signals;
    using TheOneStudio.UITemplate.UITemplate.Models.Controllers;
    using Zenject;

    public class UITemplateLeaderBoardDataController : IUITemplateControllerData
    {
        #region inject

        private readonly UITemplateLeaderBoardData uiTemplateLeaderBoardData;
        private readonly SignalBus       signalBus;

        #endregion

        public UITemplateLeaderBoardDataController(UITemplateLeaderBoardData uiTemplateLeaderBoardData, SignalBus signalBus)
        {
            this.uiTemplateLeaderBoardData = uiTemplateLeaderBoardData;
            this.signalBus       = signalBus;
        }

        public void SubmitNewScore(int newScore)
        {
            var year  = DateTime.UtcNow.Year;
            var month = DateTime.UtcNow.Month;
            var day   = DateTime.UtcNow.Day;

            //Datetime leaderboard
            var todayHighestScore = this.GetHighestDayScore(year, month, day);
            if (todayHighestScore < newScore)
            {
                this.uiTemplateLeaderBoardData.YearToMonthToDayToHighestScore[year][month][day] = newScore;
                this.signalBus.Fire(new NewDayHighestScoreSignal(todayHighestScore, newScore));
               
                if (this.GetHighestMonthScore(year, month) < newScore)
                {
                    this.signalBus.Fire(new NewMonthHighestScoreSignal(todayHighestScore, newScore));

                    if (this.GetHighestYearScore(year) < newScore)
                    {
                        this.signalBus.Fire(new NewYearHighestScoreSignal(todayHighestScore, newScore));
                    }
                } 
            }
            
            //Week Leaderboard
            var weekIndex = DateTime.UtcNow.GetWeekIndexFromStart();
            if (this.GetThisWeekHighestScore() < newScore)
            {
                this.uiTemplateLeaderBoardData.WeekIndexToHighestScore[weekIndex] = newScore;
                this.signalBus.Fire(new NewWeekHighestScoreSignal(todayHighestScore, newScore));
            }
            
            //Highest score
            if (this.uiTemplateLeaderBoardData.AllTimeHighestScore < newScore)
            {
                this.uiTemplateLeaderBoardData.AllTimeHighestScore = newScore;
                this.signalBus.Fire(new NewAllTimeHighestScoreSignal(todayHighestScore, newScore));
            }
        }

        public int GetThisWeekHighestScore()
        {
            var weekIndex = DateTime.UtcNow.GetWeekIndexFromStart();
            return this.uiTemplateLeaderBoardData.WeekIndexToHighestScore.GetOrAdd(weekIndex, () => 0);
        }

        public int GetHighestTodayScore()
        {
            var year  = DateTime.UtcNow.Year;
            var month = DateTime.UtcNow.Month;
            var day   = DateTime.UtcNow.Day;
            return this.GetHighestDayScore(year, month, day);
        }

        public int GetHighestDayScore(int year, int month, int day)
        {
            var yearData        = this.uiTemplateLeaderBoardData.YearToMonthToDayToHighestScore.GetOrAdd(year, () => new Dictionary<int, Dictionary<int, int>>());
            var monthData       = yearData.GetOrAdd(month, () => new Dictionary<int, int>());
            return monthData.GetOrAdd(day, () => 0);
        }

        public int GetHighestMonthScore(int year, int month)
        {
            var yearData  = this.uiTemplateLeaderBoardData.YearToMonthToDayToHighestScore.GetOrAdd(year, () => new Dictionary<int, Dictionary<int, int>>());
            var monthData = yearData.GetOrAdd(month, () => new Dictionary<int, int>());

            return monthData.Values.Max();
        }

        public int GetHighestYearScore(int year)
        {
            var yearData = this.uiTemplateLeaderBoardData.YearToMonthToDayToHighestScore.GetOrAdd(year, () => new Dictionary<int, Dictionary<int, int>>());
            return yearData.Values.SelectMany(monthData => monthData.Values).Max();
        }

        public int GetHighestScore()
        {
            return this.uiTemplateLeaderBoardData.AllTimeHighestScore;
        }
    }
}