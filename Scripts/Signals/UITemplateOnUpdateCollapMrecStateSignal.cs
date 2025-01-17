namespace TheOneStudio.UITemplate.UITemplate.Signals
{
    public class UITemplateOnUpdateCollapMrecStateSignal
    {
        public bool   IsActive;
        public string Placement;

        public UITemplateOnUpdateCollapMrecStateSignal(bool isActive, string placement)
        {
            this.IsActive  = isActive;
            this.Placement = placement;
        }
    }
}