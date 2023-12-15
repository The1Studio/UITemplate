namespace TheOneStudio.UITemplate.UITemplate.Leaderboard.Signals
{
    public class NewAllTimeHighScoreSignal : NewHighScoreSignal
    {
        public NewAllTimeHighScoreSignal(int oldHighScore, int newHighScore) : base(oldHighScore, newHighScore)
        {
        }
    }
}