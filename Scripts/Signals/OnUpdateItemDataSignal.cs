namespace TheOneStudio.UITemplate.UITemplate.Signals
{
    using TheOneStudio.UITemplate.UITemplate.Models;

    public class OnUpdateItemDataSignal
    {
        private readonly string rewardKey;
        private readonly string owned;
        
        public OnUpdateItemDataSignal(string rewardKey, UITemplateItemData.Status owned)
        {
            this.rewardKey = rewardKey;
            this.owned     = owned.ToString();
        }
    }
}