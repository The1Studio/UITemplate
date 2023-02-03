namespace UITemplate.Scripts.Scenes.Main
{
    using GameFoundation.Scripts.UIModule.ScreenFlow.Managers;
    using GameFoundation.Scripts.UIModule.Utilities;

    public class UITemplateMainSceneInstaller : BaseSceneInstaller
    {
        public override void InstallBindings()
        {
            base.InstallBindings();
            this.Container.InitScreenManually<UITemplateHomeSimpleScreenPresenter>();
            this.Container.InitScreenManually<UITemplateLosePresenter>();
            this.Container.InitScreenManually<UITemplateRateGamePresenter>();
            this.Container.InitScreenManually<UITemplateConnectErrorPresenter>();
        }
    }
}