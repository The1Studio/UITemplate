namespace TheOneStudio.UITemplate.UITemplate.Installers
{
    using GameFoundation.Scripts.Models;
    using GameFoundation.Scripts.Utilities;
    using TheOneStudio.UITemplate.UITemplate.Models;
    using TheOneStudio.UITemplate.UITemplate.Signals;
    using Zenject;

    public class UITemplateInstaller : Installer<UITemplateInstaller>
    {
        public override void InstallBindings()
        {
            this.Container.Bind<UITemplateUserData>().FromResolveGetter<HandleLocalDataServices>(services => services.Load<UITemplateUserData>()).AsCached().NonLazy();

            var uiTemplateUserData = this.Container.Resolve<UITemplateUserData>();

            this.Container.Bind<UITemplateLevelData>().FromInstance(uiTemplateUserData.LevelData);
            this.Container.Bind<UITemplateShopData>().FromInstance(uiTemplateUserData.ShopData);
            this.Container.Bind<UITemplateInventoryData>().FromInstance(uiTemplateUserData.InventoryData);
            this.Container.Bind<UITemplateSettingData>().FromInstance(uiTemplateUserData.SettingData);
            this.Container.Bind<UITemplateDailyRewardData>().FromInstance(uiTemplateUserData.DailyRewardData);
            this.Container.Rebind<SoundSetting>().FromInstance(uiTemplateUserData.SettingData);

            //Signal
            this.Container.DeclareSignal<UpdateCurrencySignal>();
        }
    }
}