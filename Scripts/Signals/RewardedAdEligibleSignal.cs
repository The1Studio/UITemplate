namespace TheOneStudio.UITemplate.UITemplate.Scripts.Signals
{
    using BlueprintFlow.Signals;

    public class RewardedAdEligibleSignal : ISignal
    {
        public string place;
        public RewardedAdEligibleSignal(string place) { this.place = place; }
    }
}