namespace TheOneStudio.UITemplate.UITemplate.Leaderboard.Signals
{
    public class NewYearlyHighScoreSignal : NewHighScoreSignal
    {
        public NewYearlyHighScoreSignal(int oldHighestScore, int newHighestScore) : base(oldHighestScore, newHighestScore)
        {
        }
    }
}