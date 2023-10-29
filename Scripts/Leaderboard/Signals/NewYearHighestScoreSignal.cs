namespace TheOneStudio.UITemplate.UITemplate.Leaderboard.Signals
{
    public class NewYearHighestScoreSignal : NewHighestScoreBaseSignal
    {
        public NewYearHighestScoreSignal(int oldHighestScore, int newHighestScore) : base(oldHighestScore, newHighestScore)
        {
        }
    }
}