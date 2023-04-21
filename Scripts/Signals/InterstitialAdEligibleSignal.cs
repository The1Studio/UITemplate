namespace TheOneStudio.UITemplate.UITemplate.Scripts.Signals
{
    using BlueprintFlow.Signals;
    using TheOneStudio.UITemplate.UITemplate.Signals;

    public class InterstitialAdEligibleSignal:ISignal
    {
        public string place;
        public InterstitialAdEligibleSignal(string place) { this.place = place; }
    }
}