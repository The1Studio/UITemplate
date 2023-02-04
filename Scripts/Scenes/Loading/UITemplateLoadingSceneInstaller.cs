namespace UITemplate.Scripts.Scenes.Loading
{
    using BlueprintFlow.Signals;
    using GameFoundation.Scripts.UIModule.ScreenFlow.Managers;
    using GameFoundation.Scripts.UIModule.Utilities;
    using LocalData;
    using Utility;
    using Zenject;

    public class UITemplateLoadingSceneInstaller : BaseSceneInstaller
    {
        [Inject] private UserLocalData userLocalData;

        public override void InstallBindings()
        {
            base.InstallBindings();
            this.Container.Bind<IOnlineTime>().To<WorldTimeAPI>().AsSingle().NonLazy();
            this.Container.Resolve<SignalBus>().Subscribe<LoadBlueprintDataSucceedSignal>(this.OnLoadingBlueprintSuccess);
            this.Container.InitScreenManually<UITemplateLoadingScreenPresenter>();
        }

        private async void OnLoadingBlueprintSuccess()
        {
            
        }
    }
}