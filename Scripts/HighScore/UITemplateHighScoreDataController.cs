namespace TheOneStudio.UITemplate.HighScore
{
    using System;
    using System.Linq;
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
            Enum.GetValues(typeof(HighScoreType)).Cast<HighScoreType>().ForEach(type =>
            {
                var time         = GetCurrentTime(type);
                var oldHighScore = this.highScoreData[key][type][time];
                if (newHighScore <= oldHighScore) return;

                this.highScoreData[key][type][time] = newHighScore;
                this.signalBus.Fire(new NewHighScoreSignal(type, oldHighScore, newHighScore));
            });
        }

        public int GetHighScore(string key, HighScoreType type)
        {
            return this.highScoreData[key][type][GetCurrentTime(type)];
        }

        private static DateTime GetCurrentTime(HighScoreType type) => type switch
        {
            HighScoreType.Daily   => DateTime.UtcNow.Date,
            HighScoreType.Weekly  => DateTime.UtcNow.GetFirstDayOfWeek(),
            HighScoreType.Monthly => DateTime.UtcNow.GetFirstDayOfMonth(),
            HighScoreType.Yearly  => DateTime.UtcNow.GetFirstDayOfYear(),
            HighScoreType.AllTime => DateTime.MinValue,
            _                     => throw new ArgumentOutOfRangeException(nameof(type), type, null),
        };

        #region Default

        public void SubmitScore(int newHighScore) => this.SubmitScore(DEFAULT_KEY, newHighScore);

        public int GetHighScore(HighScoreType type) => this.GetHighScore(DEFAULT_KEY, type);

        #endregion
    }
}