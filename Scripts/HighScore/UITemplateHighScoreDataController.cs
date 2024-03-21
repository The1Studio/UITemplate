namespace TheOneStudio.UITemplate.HighScore
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using GameFoundation.Scripts.Utilities.Extension;
    using TheOneStudio.UITemplate.HighScore.Models;
    using TheOneStudio.UITemplate.HighScore.Signals;
    using TheOneStudio.UITemplate.UITemplate.Models.Controllers;
    using Zenject;

    /// <summary>
    ///     Control local high score data
    /// </summary>
    public class UITemplateHighScoreDataController : IUITemplateControllerData
    {
        /// <summary>
        ///     Default Key if you don't have multiple GameModes, Characters, ...
        /// </summary>
        public const string DEFAULT_KEY = "DEFAULT";

        private const int MAX_HIGH_SCORES = 3;

        #region Constructor

        private readonly UITemplateHighScoreData highScoreData;
        private readonly SignalBus               signalBus;

        public UITemplateHighScoreDataController(UITemplateHighScoreData highScoreData, SignalBus signalBus)
        {
            this.highScoreData = highScoreData;
            this.signalBus     = signalBus;
        }

        #endregion

        /// <summary>
        ///     Submit score for all types. If a new high score reached, fire <see cref="NewHighScoreSignal"/>.
        /// </summary>
        /// <param name="key">
        ///     Use this to separate GameModes, Characters, ...
        ///     If you don't have any, use <see cref="DEFAULT_KEY"/> or <see cref="SubmitScore(int)"/>.
        /// </param>
        /// <param name="newHighScore">
        ///     Possible new high score
        /// </param>
        public void SubmitScore(string key, int newHighScore)
        {
            Enum.GetValues(typeof(HighScoreType))
                .Cast<HighScoreType>()
                .ForEach(type => this.SubmitScore(key, type, newHighScore));
        }

        /// <summary>
        ///     Submit score. If a new high score reached, fire <see cref="NewHighScoreSignal"/>.
        /// </summary>
        /// <param name="key">
        ///     Use this to separate GameModes, Characters, ...
        ///     If you don't have any, use <see cref="DEFAULT_KEY"/> or <see cref="SubmitScore(int)"/>.
        /// </param>
        /// <param name="type">
        ///     <see cref="HighScoreType"/>
        /// </param>
        /// <param name="newHighScore">
        ///     Possible new high score
        /// </param>
        public void SubmitScore(string key, HighScoreType type, int newHighScore)
        {
            var time       = GetCurrentTime(type);
            var highScores = this.highScoreData[key][type][time];
            var index      = 0;
            while (index < highScores.Count && highScores[index] > newHighScore) ++index;
            highScores.Insert(index, newHighScore);
            if (highScores.Count > MAX_HIGH_SCORES)
            {
                highScores.RemoveAt(highScores.Count - 1);
            }
            if (index == 0)
            {
                this.signalBus.Fire(new NewHighScoreSignal(key, type, highScores.Skip(1).FirstOrDefault(), newHighScore));
            }
        }

        /// <summary>
        ///     Get high score
        /// </summary>
        /// <param name="key">
        ///     Use this to separate GameModes, Characters, ...
        ///     If you don't have any, use <see cref="DEFAULT_KEY"/> or <see cref="GetHighScore(HighScoreType)"/>.
        /// </param>
        /// <param name="type">
        ///     <see cref="HighScoreType"/>
        /// </param>
        /// <returns>Highest score</returns>
        public int GetHighScore(string key, HighScoreType type)
        {
            return this.GetAllHighScores(key, type).FirstOrDefault();
        }

        /// <summary>
        ///     Get all high scores
        /// </summary>
        /// <param name="key">
        ///     Use this to separate GameModes, Characters, ...
        ///     If you don't have any, use <see cref="DEFAULT_KEY"/> or <see cref="GetHighScore(HighScoreType)"/>.
        /// </param>
        /// <param name="type">
        ///     <see cref="HighScoreType"/>
        /// </param>
        /// <returns>All high scores order from highest to lowest</returns>
        public IEnumerable<int> GetAllHighScores(string key, HighScoreType type)
        {
            return this.highScoreData[key][type][GetCurrentTime(type)];
        }

        #region Default

        /// <summary>
        ///     Submit score for all types. If a new high score reached, fire <see cref="NewHighScoreSignal"/>.
        /// </summary>
        /// <param name="newHighScore">
        ///     Possible new high score
        /// </param>
        public void SubmitScore(int newHighScore) => this.SubmitScore(DEFAULT_KEY, newHighScore);

        /// <summary>
        ///     Submit score. If a new high score reached, fire <see cref="NewHighScoreSignal"/>.
        /// </summary>
        /// <param name="type">
        ///     <see cref="HighScoreType"/>
        /// </param>
        /// <param name="newHighScore">
        ///     Possible new high score
        /// </param>
        public void SubmitScore(HighScoreType type, int newHighScore) => this.SubmitScore(DEFAULT_KEY, type, newHighScore);

        /// <summary>
        ///     Get high score
        /// </summary>
        /// <param name="type">
        ///     <see cref="HighScoreType"/>
        /// </param>
        /// <returns>Highest score</returns>
        public int GetHighScore(HighScoreType type) => this.GetHighScore(DEFAULT_KEY, type);

        /// <summary>
        ///     Get all high scores
        /// </summary>
        /// <param name="type">
        ///     <see cref="HighScoreType"/>
        /// </param>
        /// <returns>All high scores order from highest to lowest</returns>
        public IEnumerable<int> GetAllHighScores(HighScoreType type) => this.GetAllHighScores(DEFAULT_KEY, type);

        #endregion

        private static DateTime GetCurrentTime(HighScoreType type) => type switch
        {
            HighScoreType.Daily   => DateTime.UtcNow.Date,
            HighScoreType.Weekly  => DateTime.UtcNow.GetFirstDayOfWeek(),
            HighScoreType.Monthly => DateTime.UtcNow.GetFirstDayOfMonth(),
            HighScoreType.Yearly  => DateTime.UtcNow.GetFirstDayOfYear(),
            HighScoreType.AllTime => DateTime.MinValue,
            _                     => throw new ArgumentOutOfRangeException(nameof(type), type, null),
        };
    }
}