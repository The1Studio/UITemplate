namespace TheOneStudio.UITemplate.Quests.Rewards
{
    using TheOneStudio.UITemplate.UITemplate.Models.Controllers;

    public sealed class ItemReward : BaseReward
    {
        private sealed class Handler : BaseHandler<ItemReward>
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