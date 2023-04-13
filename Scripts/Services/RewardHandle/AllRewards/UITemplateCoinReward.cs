namespace TheOneStudio.UITemplate.UITemplate.Services.RewardHandle.AllRewards
{
    using System;
    using GameFoundation.Scripts.Utilities.LogService;
    using TheOneStudio.UITemplate.UITemplate.Models.Controllers;
    using UnityEngine;

    public class UITemplateCoinReward : UITemplateBaseReward
    {
        private readonly UITemplateInventoryDataController uiTemplateInventoryDataController;

        public UITemplateCoinReward(ILogService logger, UITemplateInventoryDataController uiTemplateInventoryDataController, UITemplateGetRealRewardHelper uiTemplateGetRealRewardHelper) :
            base(logger, uiTemplateGetRealRewardHelper)
        {
            this.uiTemplateInventoryDataController = uiTemplateInventoryDataController;
        }

        public override string RewardId => "Coin";

        public override void ReceiveReward(string value, RectTransform startPosAnimation)
        {
            this.uiTemplateInventoryDataController.AddCurrency(this.GetRewardValue<int>(value), this.RewardId, startPosAnimation);
            this.AfterReceiveReward(this.RewardId);
        }
    }
}