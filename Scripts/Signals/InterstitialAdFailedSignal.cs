namespace TheOneStudio.UITemplate.UITemplate.Signals
{
    public class InterstitialAdFailedSignal
    {
        public string Place;
        public string ErrorMessage;

        public InterstitialAdFailedSignal(string place, string errorMessage)
        {
            this.Place        = place;
            this.ErrorMessage = errorMessage;
        }
    }
}