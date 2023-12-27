namespace TheOneStudio.UITemplate.HighScore.Signals
{
    public class NewMonthlyHighScoreSignal : NewHighScoreSignal
    {
        public NewMonthlyHighScoreSignal(int oldHighScore, int newHighScore) : base(oldHighScore, newHighScore)
        {
        }
    }
}