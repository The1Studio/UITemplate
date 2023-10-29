namespace TheOneStudio.UITemplate.UITemplate.Leaderboard.Signals
{
    public class NewDayHighestScoreSignal : NewHighestScoreBaseSignal
    {
        public NewDayHighestScoreSignal(int oldHighestScore, int newHighestScore) : base(oldHighestScore, newHighestScore)
        {
        }
    }
}