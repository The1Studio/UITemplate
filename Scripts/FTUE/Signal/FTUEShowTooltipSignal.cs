namespace TheOneStudio.UITemplate.UITemplate.FTUE.Signal
{
    using UnityEngine;

    public class FTUEShowTooltipSignal
    {
        public string     FTUEId          { get; set; }
        public GameObject HighlightObject { get; set; }

        public FTUEShowTooltipSignal(string ftueId, GameObject highlightObject)
        {
            this.FTUEId          = ftueId;
            this.HighlightObject = highlightObject;
        }
    }
}