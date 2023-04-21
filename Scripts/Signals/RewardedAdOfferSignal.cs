namespace TheOneStudio.UITemplate.UITemplate.Signals
{
    using BlueprintFlow.Signals;

    public class RewardedAdOfferSignal:ISignal
    {
        public string Place;

        public RewardedAdOfferSignal(string place) { this.Place = place; }
    }
}