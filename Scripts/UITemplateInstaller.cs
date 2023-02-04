namespace UITemplate.Scripts
{
    using UITemplate.Scripts.Models;
    using UITemplate.Scripts.Signals;
    using Zenject;

    public class UITemplateInstaller : Installer<UITemplateInstaller>
    {
        public override void InstallBindings()
        {
            //Bind Models
            var uiTemplateUserData = this.Container.Instantiate<UITemplateUserData>();
            this.Container.Bind<UITemplateUserData>().FromInstance(uiTemplateUserData);
            this.Container.Bind<UITemplateLevelData>().FromInstance(uiTemplateUserData.LevelData);
            this.Container.Bind<UITemplateShopData>().FromInstance(uiTemplateUserData.ShopData);
            this.Container.Bind<UITemplateInventoryData>().FromInstance(uiTemplateUserData.InventoryData);
            this.Container.Bind<UITemplateSettingData>().FromInstance(uiTemplateUserData.SettingData);

            //Signal
            this.Container.DeclareSignal<UpdateCurrencySignal>();
        }
    }
}