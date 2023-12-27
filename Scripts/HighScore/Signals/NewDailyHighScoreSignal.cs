namespace TheOneStudio.UITemplate.HighScore.Signals
{
    public class NewDailyHighScoreSignal : NewHighScoreSignal
    {
        public NewDailyHighScoreSignal(int oldHighScore, int newHighScore) : base(oldHighScore, newHighScore)
        {
        }
    }
}