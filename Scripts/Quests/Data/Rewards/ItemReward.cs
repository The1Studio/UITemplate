namespace TheOneStudio.UITemplate.Quests.Data.Rewards
{
    using TheOneStudio.UITemplate.UITemplate.Models.Controllers;

    public class ItemReward : Reward
    {
        private class Handler : RewardHandler<ItemReward>
        {
            private readonly UITemplateInventoryDataController inventoryDataController;

            public Handler(UITemplateInventoryDataController inventoryDataController)
            {
                this.inventoryDataController = inventoryDataController;
            }

            protected override void Handle(ItemReward reward)
            {
                var item = this.inventoryDataController.GetItemData(reward.Id);
                this.inventoryDataController.SetOwnedItemData(item);
            }
        }
    }
}