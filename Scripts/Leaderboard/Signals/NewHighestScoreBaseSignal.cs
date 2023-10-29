namespace TheOneStudio.UITemplate.UITemplate.Leaderboard.Signals
{
    public class NewHighestScoreBaseSignal
    {
        public int OldHighestScore;
        public int NewHighestScore;
        
        public NewHighestScoreBaseSignal(int oldHighestScore, int newHighestScore)
        {
            this.OldHighestScore = oldHighestScore;
            this.NewHighestScore = newHighestScore;
        }
    }
}