namespace UITemplate.Scripts.Models
{
    using UITemplate.Scripts.Blueprints;

    public class UITemplateUserData
    {
        public readonly UITemplateLevelData     LevelData;
        public readonly UITemplateShopData      ShopData;
        public readonly UITemplateInventoryData InventoryData;
        public readonly UITemplateSettingData   SettingData;

        public UITemplateUserData(UITemplateShopBlueprint uiTemplateShopBlueprint, UITemplateLevelBlueprint uiTemplateLevelBlueprint)
        {
            this.LevelData     = new UITemplateLevelData(uiTemplateLevelBlueprint);
            this.ShopData      = new UITemplateShopData(uiTemplateShopBlueprint);
            this.InventoryData = new UITemplateInventoryData();
            this.SettingData   = new UITemplateSettingData();
        }
    }
}