namespace TheOneStudio.UITemplate.UITemplate.Signals
{
    using BlueprintFlow.Signals;

    public class UpdateCurrencySignal:ISignal
    {
        public string Id;
        public int    Amount;
        public int    FinalValue;
        
        public UpdateCurrencySignal()
        {
        }
    }
}