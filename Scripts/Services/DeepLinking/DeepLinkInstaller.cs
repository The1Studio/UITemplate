namespace TheOneStudio.UITemplate.UITemplate.Services.DeepLinking
{
    using Zenject;

    public class DeepLinkInstaller : Installer<DeepLinkInstaller>
    {
        public override void InstallBindings()
        {
            this.Container.BindInitializableExecutionOrder<DeepLinkService>(int.MinValue).NonLazy();
            this.Container.DeclareSignal<OnDeepLinkActiveSignal>();
        }
    }
}