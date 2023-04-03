namespace TheOneStudio.UITemplate.UITemplate.FTUE.Signal
{
    public class FTUEButtonClickSignal
    {
        public string ButtonId { get; set; }
        public string FTUEId   { get; set; }

        public FTUEButtonClickSignal(string buttonId, string ftueId)
        {
            this.ButtonId = buttonId;
            this.FTUEId   = ftueId;
        }
    }
}