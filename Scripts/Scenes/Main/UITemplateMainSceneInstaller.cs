using UITemplate.Scripts.Scenes.Popups;
using UITemplate.Scripts.Signals;
using Zenject;

namespace UITemplate.Scripts.Scenes.Main
{
    using GameFoundation.Scripts.UIModule.ScreenFlow.Managers;
    using GameFoundation.Scripts.UIModule.Utilities;
    using UITemplate.Scripts.Models;

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
        }
    }
}