namespace UITemplate.Scripts.Models
{
    using UITemplate.Scripts.Blueprints;
    using UniRx;

    public class UITemplateUserData
    {
        public readonly UITemplateLevelData     LevelData;
        public readonly UITemplateShopData      ShopData;
        public readonly UITemplateInventoryData InventoryData;
        public readonly UITemplateSettingData   SettingData;
        public          UserPackageData         UserPackageData = new();

        public UITemplateUserData(UITemplateShopBlueprint uiTemplateShopBlueprint, UITemplateLevelBlueprint uiTemplateLevelBlueprint)
        {
            this.LevelData     = new UITemplateLevelData(uiTemplateLevelBlueprint);
            this.ShopData      = new UITemplateShopData(uiTemplateShopBlueprint);
            this.InventoryData = new UITemplateInventoryData();
            this.SettingData   = new UITemplateSettingData();
            this.UserPackageData = new UserPackageData
            {
                CurrentSelectCharacterId = new StringReactiveProperty("default_character")
            };
        }
    }

    public class UserPackageData
    {
        public StringReactiveProperty CurrentSelectCharacterId { get; set; }
    }
}