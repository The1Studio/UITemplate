namespace TheOneStudio.UITemplate.HighScore.Signals
{
    using TheOneStudio.UITemplate.HighScore.Models;

    public sealed class NewHighScoreSignal
    {
        public HighScoreType Type         { get; }
        public int           OldHighScore { get; }
        public int           NewHighScore { get; }

        public NewHighScoreSignal(HighScoreType type, int oldHighScore, int newHighScore)
        {
            this.Type         = type;
            this.OldHighScore = oldHighScore;
            this.NewHighScore = newHighScore;
        }
    }
}