namespace TheOneStudio.UITemplate.UITemplate.Signals
{
    using System.Collections.Generic;

    public class OnUpdateCurrencySignal
    {
        public string                     Id;
        public int                        Amount;
        public int                        FinalValue;
        public string                     Source;
        public Dictionary<string, object> Metadata;

        public OnUpdateCurrencySignal(string id, int amount, int finalValue, string source = null, Dictionary<string, object> metadata = null)
        {
            this.Id         = id;
            this.Amount     = amount;
            this.FinalValue = finalValue;
            this.Source     = source;
            this.Metadata   = metadata;
        }
    }
}