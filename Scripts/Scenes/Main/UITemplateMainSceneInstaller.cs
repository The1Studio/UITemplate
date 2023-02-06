using UITemplate.Scripts.Scenes.Popups;
using UITemplate.Scripts.Signals;
using Zenject;

namespace UITemplate.Scripts.Scenes.Main
{
    using GameFoundation.Scripts.UIModule.ScreenFlow.Managers;
    using GameFoundation.Scripts.UIModule.Utilities;
    using UITemplate.Scripts.Models;
    using UITemplate.Scripts.Signals;
    using Zenject;

    public class UITemplateMainSceneInstaller : BaseSceneInstaller
    {
        public override void InstallBindings()
        {
            base.InstallBindings();
            this.DeclareSignals();
            this.Container.Bind<UITemplateSettingData>().AsSingle().NonLazy();
            this.Container.InitScreenManually<UITemplateHomeSimpleScreenPresenter>();
            
        }
        
        private void DeclareSignals()
        {
            this.Container.DeclareSignal<UpdateCurrencySignal>();
            
            var uiTemplateUserData = this.Container.Instantiate<UITemplateUserData>();
            this.Container.Bind<UITemplateUserData>().FromInstance(uiTemplateUserData);
            this.Container.Bind<UITemplateLevelData>().FromInstance(uiTemplateUserData.LevelData);
            this.Container.Bind<UITemplateShopData>().FromInstance(uiTemplateUserData.ShopData);
            this.Container.Bind<UITemplateInventoryData>().FromInstance(uiTemplateUserData.InventoryData);
            this.Container.Bind<UITemplateSettingData>().FromInstance(uiTemplateUserData.SettingData);
        }
    }
}