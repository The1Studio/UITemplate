namespace TheOneStudio.UITemplate.UITemplate.Signals
{
    public class UpdateCurrencySignal
    {
        public string Id;
        public int    Amount;
        public int    FinalValue;

        public UpdateCurrencySignal(string id, int amount, int finalValue)
        {
            this.Id         = id;
            this.Amount     = amount;
            this.FinalValue = finalValue;
        }
    }
}