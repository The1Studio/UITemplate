namespace TheOneStudio.UITemplate.UITemplate.FTUE.Signal
{
    using BlueprintFlow.Signals;

    public class FTUETriggerSignal : ISignal
    {
        public string StepId { get; set; }

        public FTUETriggerSignal(string stepId) { this.StepId = stepId; }
    }
}