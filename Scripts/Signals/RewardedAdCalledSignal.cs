namespace TheOneStudio.UITemplate.UITemplate.Signals
{
    using BlueprintFlow.Signals;

    public class RewardedAdCalledSignal:ISignal
    {
        public string place;
        public RewardedAdCalledSignal(string place) { this.place = place; }
    }
}