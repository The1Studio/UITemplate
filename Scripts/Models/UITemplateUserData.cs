namespace UITemplate.Scripts.Models
{
    using GameFoundation.Scripts.Interfaces;
    using UITemplate.Scripts.Blueprints;
    using UniRx;

    public class UITemplateUserData : ILocalData
    {

        public readonly UITemplateLevelData     LevelData;
        public readonly UITemplateShopData      ShopData;
        public readonly UITemplateInventoryData InventoryData;
        public readonly UITemplateSettingData   SettingData;
        public          UserPackageData         UserPackageData = new();


        public UITemplateUserData(UITemplateShopBlueprint uiTemplateShopBlueprint, UITemplateLevelBlueprint uiTemplateLevelBlueprint)
        {
            this.LevelData       = new UITemplateLevelData(uiTemplateLevelBlueprint);
            this.ShopData        = new UITemplateShopData(uiTemplateShopBlueprint);
            this.InventoryData   = new UITemplateInventoryData();
            this.SettingData     = new UITemplateSettingData();
            this.DailyRewardData = new UITemplateDailyRewardData();
        }
        
        public UITemplateUserData()
        {
        }
        
        public void Init()
        {
        }
    }

    public class UserPackageData
    {
        public StringReactiveProperty CurrentSelectCharacterId { get; set; }
        public StringReactiveProperty CurrentSelectItemId      { get; set; }
    }
}