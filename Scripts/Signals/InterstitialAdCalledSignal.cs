namespace TheOneStudio.UITemplate.UITemplate.Signals
{
    using BlueprintFlow.Signals;

    public class InterstitialAdCalledSignal:ISignal
    {
        public string place;
        public InterstitialAdCalledSignal(string place) { this.place = place; }
    }
}