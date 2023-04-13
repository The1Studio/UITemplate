namespace TheOneStudio.UITemplate.UITemplate.Services.RewardHandle.AllRewards
{
    using System.Linq;
    using GameFoundation.Scripts.Utilities.Extension;
    using GameFoundation.Scripts.Utilities.LogService;
    using TheOneStudio.UITemplate.UITemplate.Blueprints;
    using TheOneStudio.UITemplate.UITemplate.Models;
    using TheOneStudio.UITemplate.UITemplate.Models.Controllers;
    using UnityEngine;

    public class UITemplateItemReward : UITemplateBaseReward
    {
        private readonly UITemplateShopBlueprint           uiTemplateShopBlueprint;
        private readonly UITemplateInventoryDataController uiTemplateInventoryDataController;
        private readonly UITemplateItemBlueprint           uiTemplateItemBlueprint;

        public UITemplateItemReward(ILogService logger, UITemplateShopBlueprint uiTemplateShopBlueprint,
            UITemplateInventoryDataController uiTemplateInventoryDataController,
            UITemplateItemBlueprint uiTemplateItemBlueprint, UITemplateGetRealRewardHelper uiTemplateGetRealRewardHelper) : base(logger, uiTemplateGetRealRewardHelper)
        {
            this.uiTemplateShopBlueprint           = uiTemplateShopBlueprint;
            this.uiTemplateInventoryDataController = uiTemplateInventoryDataController;
            this.uiTemplateItemBlueprint           = uiTemplateItemBlueprint;
        }

        public override string RewardId => "Item";

        public override void ReceiveReward(string value, RectTransform startPosAnimation)
        {
            var finalItem = value;

            if (value.Equals("Random") || value.Equals("random"))
            {
                var listAllItemAvailable = this.uiTemplateInventoryDataController.GetAllItemAvailable();

                if (listAllItemAvailable.Count > 0)
                {
                    var listCanRandom = (from itemData in listAllItemAvailable
                        where this.uiTemplateItemBlueprint.ContainsKey(itemData.Value.Id) && this.uiTemplateShopBlueprint.ContainsKey(itemData.Value.Id)
                        select itemData.Value.Id).ToList();

                    if (listCanRandom.Count > 0)
                    {
                        finalItem = listCanRandom.PickRandom();
                    }
                }
            }

            this.Logger.LogWithColor($"Final Item Owned: {finalItem}", Color.cyan);

            this.uiTemplateInventoryDataController.AddItemData(new UITemplateItemData(finalItem, this.uiTemplateShopBlueprint[finalItem], this.uiTemplateItemBlueprint[finalItem],
                UITemplateItemData.Status.Owned));
            this.AfterReceiveReward(finalItem);
        }
    }
}