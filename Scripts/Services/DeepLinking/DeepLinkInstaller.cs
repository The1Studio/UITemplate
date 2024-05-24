namespace TheOneStudio.UITemplate.UITemplate.Services.DeepLinking
{
    using Zenject;

    public class DeepLinkInstaller : Installer<DeepLinkInstaller>
    {
        public override void InstallBindings()
        {
            this.Container.BindInterfacesTo<DeepLinkService>().AsSingle().NonLazy();
            this.Container.DeclareSignal<OnDeepLinkActiveSignal>();
        }
    }
}