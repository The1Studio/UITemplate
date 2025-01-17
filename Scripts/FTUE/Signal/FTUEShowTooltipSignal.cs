namespace TheOneStudio.UITemplate.UITemplate.FTUE.Signal
{
    using UnityEngine;

    public class FTUEShowTooltipSignal
    {
        public string     FTUEId          { get; set; }
        public GameObject HighlightObject { get; set; }
        public float      Duration        { get; set; }

        public FTUEShowTooltipSignal(string ftueId, GameObject highlightObject, float duration)
        {
            this.FTUEId          = ftueId;
            this.HighlightObject = highlightObject;
            this.Duration        = duration;
        }
    }
}