namespace TheOneStudio.UITemplate.UITemplate.FTUE.Signal
{
    public class FTUEButtonClickSignal
    {
        public string ButtonId { get; set; }
        public string StepId   { get; set; }

        public FTUEButtonClickSignal(string buttonId, string stepId)
        {
            this.ButtonId = buttonId;
            this.StepId   = stepId;
        }
    }
}