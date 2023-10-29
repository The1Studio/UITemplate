namespace TheOneStudio.UITemplate.UITemplate.Leaderboard.Signals
{
    public class NewWeekHighestScoreSignal : NewHighestScoreBaseSignal
    {
        public NewWeekHighestScoreSignal(int oldHighestScore, int newHighestScore) : base(oldHighestScore, newHighestScore)
        {
        }
    }
}