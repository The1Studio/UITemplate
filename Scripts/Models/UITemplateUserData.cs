namespace UITemplate.Scripts.Models
{
    using UITemplate.Scripts.Blueprints;

    public class UITemplateUserData
    {
        public readonly  UITemplateLevelData      LevelData;
        public readonly  UITemplateShopData       ShopData;

        public UITemplateUserData(UITemplateShopBlueprint uiTemplateShopBlueprint, UITemplateLevelBlueprint uiTemplateLevelBlueprint)
        {
            this.ShopData                  = new UITemplateShopData(uiTemplateShopBlueprint);
            this.LevelData                 = new UITemplateLevelData(uiTemplateLevelBlueprint);
        }
    }
}