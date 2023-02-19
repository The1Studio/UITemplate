namespace TheOneStudio.UITemplate.UITemplate.Models
{
    using GameFoundation.Scripts.Interfaces;
    using Zenject;

    public class UITemplateUserData : ILocalData
    {
        public readonly UITemplateLevelData       LevelData;
        public readonly UITemplateShopData        ShopData;
        public readonly UITemplateInventoryData   InventoryData;
        public readonly UITemplateSettingData     SettingData;
        public readonly UITemplateDailyRewardData DailyRewardData;

        public UITemplateUserData(DiContainer diContainer)
        {
            this.LevelData       = diContainer.Instantiate<UITemplateLevelData>();
            this.ShopData        = diContainer.Instantiate<UITemplateShopData>();
            this.InventoryData   = diContainer.Instantiate<UITemplateInventoryData>();
            this.SettingData     = diContainer.Instantiate<UITemplateSettingData>();
            this.DailyRewardData = diContainer.Instantiate<UITemplateDailyRewardData>();
        }

        public void Init() { }
    }
}