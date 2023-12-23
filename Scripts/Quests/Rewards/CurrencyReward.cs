namespace TheOneStudio.UITemplate.Quests.Rewards
{
    using Cysharp.Threading.Tasks;
    using TheOneStudio.UITemplate.UITemplate.Models.Controllers;

    public sealed class CurrencyReward : BaseReward
    {
        private sealed class Handler : BaseHandler<CurrencyReward>
        {
            private readonly UITemplateInventoryDataController inventoryDataController;

            public Handler(UITemplateInventoryDataController inventoryDataController)
            {
                this.inventoryDataController = inventoryDataController;
            }

            protected override void Handle(CurrencyReward reward)
            {
                this.inventoryDataController.AddCurrency(reward.Value, reward.Id).Forget();
            }
        }
    }
}