namespace TheOneStudio.UITemplate.UITemplate.Leaderboard.Signals
{
    public class NewAllTimeHighestScoreSignal : NewHighestScoreBaseSignal
    {
        public NewAllTimeHighestScoreSignal(int oldHighestScore, int newHighestScore) : base(oldHighestScore, newHighestScore)
        {
        }
    }
}