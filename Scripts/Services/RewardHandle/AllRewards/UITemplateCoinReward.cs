namespace TheOneStudio.UITemplate.UITemplate.Services.RewardHandle.AllRewards
{
    using GameFoundation.Scripts.Utilities.LogService;
    using TheOneStudio.UITemplate.UITemplate.Models.Controllers;

    public class UITemplateCoinReward : UITemplateBaseReward
    {
        private readonly UITemplateInventoryDataController uiTemplateInventoryDataController;

        public UITemplateCoinReward(ILogService logger, UITemplateInventoryDataController uiTemplateInventoryDataController) : base(logger)
        {
            this.uiTemplateInventoryDataController = uiTemplateInventoryDataController;
        }

        public override string RewardId => "Coin";

        public override void ReceiveReward(string value,string addressableFlyingItem)
        {
            this.uiTemplateInventoryDataController.AddCurrency(this.GetRewardValue<int>(value), this.RewardId, startAnimationRect: this.StartPosAnimation);
        }
    }
}