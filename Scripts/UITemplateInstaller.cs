namespace UITemplate.Scripts
{
    using UITemplate.Scripts.Models;
    using Zenject;

    public class UITemplateInstaller : Installer<UITemplateInstaller>
    {
        public override void InstallBindings()
        {
            //Bind Models
            var uiTemplateUserData = this.Container.Instantiate<UITemplateUserData>();
            this.Container.Bind<UITemplateUserData>().FromInstance(uiTemplateUserData);
            this.Container.Bind<UITemplateLevelData>().FromInstance(uiTemplateUserData.LevelData);
            // public readonly UITemplateShopData      ShopData;
            // public readonly UITemplateInventoryData InventoryData;
            // public readonly UITemplateSettingData   SettingData;
        }
    }
}