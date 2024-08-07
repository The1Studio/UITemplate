namespace TheOneStudio.UITemplate.UITemplate.Services.Permissions
{
    using TheOneStudio.UITemplate.UITemplate.Services.Permissions.Signals;
    using Zenject;

    public class PermissionServiceInstaller : Installer<PermissionServiceInstaller>
    {
        public override void InstallBindings()
        {
            this.Container.BindInterfacesTo<PermissionService>().AsCached().Lazy();

            this.Container.DeclareSignal<OnRequestPermissionStartSignal>();
            this.Container.DeclareSignal<OnRequestPermissionCompleteSignal>();
        }
    }
}