namespace TheOneStudio.UITemplate.Quests.Data.Rewards
{
    using Cysharp.Threading.Tasks;
    using TheOneStudio.UITemplate.UITemplate.Models.Controllers;

    public class CurrencyReward : Reward
    {
        private class Handler : RewardHandler<CurrencyReward>
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