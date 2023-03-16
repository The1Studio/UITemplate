namespace TheOneStudio.UITemplate.UITemplate.Signals
{
    public class RewardedAdFailedSignal
    {
        public string Place;
        public string ErrorMessage;

        public RewardedAdFailedSignal(string place, string errorMessage)
        {
            this.ErrorMessage = errorMessage;
            this.Place        = place;
        }
    }
}