namespace TheOneStudio.UITemplate.UITemplate.Leaderboard.Signals
{
    public class NewWeeklyHighScoreSignal : NewHighScoreSignal
    {
        public NewWeeklyHighScoreSignal(int oldHighScore, int newHighScore) : base(oldHighScore, newHighScore)
        {
        }
    }
}