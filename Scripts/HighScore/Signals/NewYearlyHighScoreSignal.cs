namespace TheOneStudio.UITemplate.HighScore.Signals
{
    public class NewYearlyHighScoreSignal : NewHighScoreSignal
    {
        public NewYearlyHighScoreSignal(int oldHighestScore, int newHighestScore) : base(oldHighestScore, newHighestScore)
        {
        }
    }
}