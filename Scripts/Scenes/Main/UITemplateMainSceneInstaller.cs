namespace UITemplate.Scripts.Scenes.Main
{
    using GameFoundation.Scripts.UIModule.ScreenFlow.Managers;
    using GameFoundation.Scripts.UIModule.Utilities;
    using UITemplate.Scripts.Signals;
    using Zenject;

    public class UITemplateMainSceneInstaller : BaseSceneInstaller
    {
        public override void InstallBindings()
        {
            base.InstallBindings();
            this.Container.InitScreenManually<UITemplateHomeSimpleScreenPresenter>();
            this.Container.DeclareSignal<UpdateCurrencySignal>();
        }
    }
}