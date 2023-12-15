namespace TheOneStudio.UITemplate.UITemplate.Leaderboard.Signals
{
    public abstract class NewHighScoreSignal
    {
        public int OldHighScore { get; }
        public int NewHighScore { get; }

        protected NewHighScoreSignal(int oldHighScore, int newHighScore)
        {
            this.OldHighScore = oldHighScore;
            this.NewHighScore = newHighScore;
        }
    }
}