namespace TheOneStudio.UITemplate.HighScore.Signals
{
    using TheOneStudio.UITemplate.HighScore.Models;

    /// <summary>
    ///     Fire when reached a new high score
    /// </summary>
    public sealed class NewHighScoreSignal
    {
        public string        Key          { get; }
        public HighScoreType Type         { get; }
        public int           OldHighScore { get; }
        public int           NewHighScore { get; }

        internal NewHighScoreSignal(string key, HighScoreType type, int oldHighScore, int newHighScore)
        {
            this.Key          = key;
            this.Type         = type;
            this.OldHighScore = oldHighScore;
            this.NewHighScore = newHighScore;
        }
    }
}