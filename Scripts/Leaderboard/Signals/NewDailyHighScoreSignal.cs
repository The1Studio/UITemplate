namespace TheOneStudio.UITemplate.UITemplate.Leaderboard.Signals
{
    public class NewDailyHighScoreSignal : NewHighScoreSignal
    {
        public NewDailyHighScoreSignal(int oldHighScore, int newHighScore) : base(oldHighScore, newHighScore)
        {
        }
    }
}