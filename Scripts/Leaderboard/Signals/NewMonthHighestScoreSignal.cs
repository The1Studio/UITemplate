namespace TheOneStudio.UITemplate.UITemplate.Leaderboard.Signals
{
    public class NewMonthHighestScoreSignal : NewHighestScoreBaseSignal
    {
        public NewMonthHighestScoreSignal(int oldHighestScore, int newHighestScore) : base(oldHighestScore, newHighestScore)
        {
        }
    }
}