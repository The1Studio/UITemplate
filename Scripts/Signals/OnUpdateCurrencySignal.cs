namespace TheOneStudio.UITemplate.UITemplate.Signals
{
    public class OnUpdateCurrencySignal
    {
        public string Id;
        public int    Amount;
        public int    FinalValue;

        public OnUpdateCurrencySignal(string id, int amount, int finalValue)
        {
            this.Id         = id;
            this.Amount     = amount;
            this.FinalValue = finalValue;
        }
    }
}