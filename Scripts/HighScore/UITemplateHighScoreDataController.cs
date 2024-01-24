namespace TheOneStudio.UITemplate.HighScore
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using GameFoundation.Scripts.Utilities.Extension;
    using TheOneStudio.UITemplate.HighScore.Models;
    using TheOneStudio.UITemplate.HighScore.Signals;
    using TheOneStudio.UITemplate.UITemplate.Models.Controllers;
    using UnityEngine;
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
        ///     Submit score. If a new high score reached, fire <see cref="NewHighScoreSignal"/>.
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
            Enum.GetValues(typeof(HighScoreType)).Cast<HighScoreType>().ForEach(type =>
            {
                var time         = GetCurrentTime(type);
                var oldHighScore = this.highScoreData[key][type][time];
                if (newHighScore <= oldHighScore) return;

                this.highScoreData[key][type][time] = newHighScore;
                this.signalBus.Fire(new NewHighScoreSignal(key, type, oldHighScore, newHighScore));
            });
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
        /// <returns>High score</returns>
        public int GetHighScore(string key, HighScoreType type)
        {
            return this.highScoreData[key][type][GetCurrentTime(type)];
        }

        public void SetLastHighScore(string key, int newHighScore)
        {
            Enum.GetValues(typeof(HighScoreType)).Cast<HighScoreType>().ForEach(type =>
            {
                this.highScoreData[key].AddAllHighScore(type,DateTime.UtcNow,newHighScore);
            });
            
        }
        public Dictionary<DateTime,int> GetAllHighScore(string key, HighScoreType type)
        {
            return this.highScoreData[key].GetAllHighScore(type).topUserHighScores;
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

        /// <summary>
        ///     Submit score. If a new high score reached, fire <see cref="NewHighScoreSignal" />.
        /// </summary>
        /// <param name="newHighScore">
        ///     Possible new high score
        /// </param>
        public void SubmitScore(int newHighScore) => this.SubmitScore(DEFAULT_KEY, newHighScore);

        /// <summary>
        ///     Get high score
        /// </summary>
        /// <param name="type">
        ///     <see cref="HighScoreType"/>
        /// </param>
        /// <returns>High score</returns>
        public int GetHighScore(HighScoreType type) => this.GetHighScore(DEFAULT_KEY, type);

        #endregion
    }
}