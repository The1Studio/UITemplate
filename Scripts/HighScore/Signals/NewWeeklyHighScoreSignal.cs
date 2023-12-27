namespace TheOneStudio.UITemplate.HighScore.Signals
{
    public class NewWeeklyHighScoreSignal : NewHighScoreSignal
    {
        public NewWeeklyHighScoreSignal(int oldHighScore, int newHighScore) : base(oldHighScore, newHighScore)
        {
        }
    }
}