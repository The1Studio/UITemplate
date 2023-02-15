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
                CurrentSelectCharacterId = new StringReactiveProperty("Play_Caracter_Sample03"),
                CurrentSelectItemId      = new StringReactiveProperty("Icon_Adremove")
            };

            this.ShopData.UpdateStatusItemData("Play_Caracter_Sample0", ItemData.Status.Unlocked);
            this.ShopData.UpdateStatusItemData("Play_Caracter_Sample1", ItemData.Status.Unlocked);
            this.ShopData.UpdateStatusItemData("Play_Caracter_Sample03", ItemData.Status.Owned);
            this.ShopData.UpdateStatusItemData("Play_Caracter_Sample04", ItemData.Status.Owned);
            this.ShopData.UpdateStatusItemData("Play_Caracter_Sample05", ItemData.Status.Owned);
            this.ShopData.UpdateStatusItemData("Play_Caracter_Sample06", ItemData.Status.Owned);
            this.ShopData.UpdateStatusItemData("Play_Caracter_Sample07", ItemData.Status.Owned);
            this.ShopData.UpdateStatusItemData("Icon_Adremove", ItemData.Status.Unlocked);
            this.ShopData.UpdateStatusItemData("Icon_Bag", ItemData.Status.Unlocked);
        }
    }

    public class UserPackageData
    {
        public StringReactiveProperty CurrentSelectCharacterId { get; set; }
        public StringReactiveProperty CurrentSelectItemId      { get; set; }
    }
}