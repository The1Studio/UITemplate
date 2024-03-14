namespace TheOneStudio.UITemplate.UITemplate.Signals
{
    public class UITemplateOnUpdateBannerStateSignal
    {
        public bool IsActive;
        
        public UITemplateOnUpdateBannerStateSignal(bool isActive)
        {
            this.IsActive = isActive;
        }
    }
}