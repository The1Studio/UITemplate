namespace TheOneStudio.UITemplate.UITemplate.FTUE.Signal
{
    using UnityEngine;

    public class FTUEShowTooltipSignal
    {
        public string     TooltipText     { get; set; }
        public GameObject HighlightObject { get; set; }
        public float      Duration        { get; set; }

        public FTUEShowTooltipSignal(string tooltipText, GameObject highlightObject, float duration)
        {
            this.TooltipText     = tooltipText;
            this.HighlightObject = highlightObject;
            this.Duration        = duration;
        }
    }
}