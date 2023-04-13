namespace TheOneStudio.UITemplate.UITemplate.Services.RewardHandle.AllRewards
{
    using GameFoundation.Scripts.Utilities.Extension;
    using GameFoundation.Scripts.Utilities.LogService;
    using TheOneStudio.UITemplate.UITemplate.Blueprints;
    using TheOneStudio.UITemplate.UITemplate.Models;
    using TheOneStudio.UITemplate.UITemplate.Models.Controllers;

    public class UITemplateItemReward : UITemplateBaseReward
    {
        private readonly UITemplateFlyingAnimationCurrency flyingAnimationCurrency;
        private readonly UITemplateShopBlueprint           uiTemplateShopBlueprint;
        private readonly UITemplateInventoryDataController uiTemplateInventoryDataController;
        private readonly UITemplateItemBlueprint           uiTemplateItemBlueprint;

        public UITemplateItemReward(ILogService logger, UITemplateFlyingAnimationCurrency flyingAnimationCurrency, UITemplateShopBlueprint uiTemplateShopBlueprint,
            UITemplateInventoryDataController uiTemplateInventoryDataController,
            UITemplateItemBlueprint uiTemplateItemBlueprint) : base(logger)
        {
            this.flyingAnimationCurrency           = flyingAnimationCurrency;
            this.uiTemplateShopBlueprint           = uiTemplateShopBlueprint;
            this.uiTemplateInventoryDataController = uiTemplateInventoryDataController;
            this.uiTemplateItemBlueprint           = uiTemplateItemBlueprint;
        }

        public override string RewardId => "Item";

        public override void ReceiveReward(string value, string addressableFlyingItem)
        {
            var finalItem = value;

            if (value.Equals("Random") || value.Equals("random"))
            {
                if (this.uiTemplateItemBlueprint.Count > 0)
                {
                    finalItem = this.uiTemplateItemBlueprint.PickRandom().Value.Id;
                }
            }

            this.uiTemplateInventoryDataController.AddItemData(new UITemplateItemData(finalItem, this.uiTemplateShopBlueprint[finalItem], this.uiTemplateItemBlueprint[finalItem],
                UITemplateItemData.Status.Owned));

            //flyingAnimation
            if (string.IsNullOrEmpty(addressableFlyingItem)) return;

            if (this.StartPosAnimation != null)
            {
                this.flyingAnimationCurrency.PlayAnimation(this.StartPosAnimation, 1, 1, target: null, prefabName: addressableFlyingItem);
            }
        }
    }
}