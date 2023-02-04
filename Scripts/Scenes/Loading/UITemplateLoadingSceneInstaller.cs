namespace UITemplate.Scripts.Scenes.Loading
{
    using GameFoundation.Scripts.UIModule.ScreenFlow.Managers;
    using GameFoundation.Scripts.UIModule.Utilities;
    using Utility;

    public class UITemplateLoadingSceneInstaller : BaseSceneInstaller
    {

        public override void InstallBindings()
        {
            base.InstallBindings();
            this.Container.Bind<IInternetService>().To<InternetService>().AsSingle().NonLazy();
            this.Container.InitScreenManually<UITemplateLoadingScreenPresenter>();
        }
    }
}