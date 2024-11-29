namespace TheOneStudio.UITemplate.UITemplate.Signals
{
    public class OnUpdateCurrencySignal
    {
        public string Id;
        public int    Amount;
        public int    FinalValue;
        public string Source;

        public OnUpdateCurrencySignal(string id, int amount, int finalValue, string source = null)
        {
            this.Id         = id;
            this.Amount     = amount;
            this.FinalValue = finalValue;
            this.Source     = source;
        }
    }
}