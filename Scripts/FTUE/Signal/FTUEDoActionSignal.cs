namespace TheOneStudio.UITemplate.UITemplate.FTUE.Signal
{
    public class FTUEDoActionSignal : IHaveStepId
    {
        public string StepId { get; set; }
        
        public FTUEDoActionSignal(string stepId) { this.StepId = stepId; }
    }
}