namespace TheOneStudio.UITemplate.UITemplate.Leaderboard.Signals
{
    public class NewMonthlyHighScoreSignal : NewHighScoreSignal
    {
        public NewMonthlyHighScoreSignal(int oldHighScore, int newHighScore) : base(oldHighScore, newHighScore)
        {
        }
    }
}