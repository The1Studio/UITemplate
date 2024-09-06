#if GDK_ZENJECT
namespace TheOneStudio.UITemplate.UITemplate.Creative
{
    using TheOneStudio.UITemplate.UITemplate.Creative.CheatLevel;
    using TheOneStudio.UITemplate.UITemplate.Scripts.Services;
    using Zenject;

    public class NewCreativeServiceInstaller : Installer<NewCreativeServiceInstaller>
    {
        public override void InstallBindings()
        {
#if !CREATIVE
            return;
#endif
            this.Container.BindInterfacesAndSelfTo<NewCreativeService>().AsCached().NonLazy();

            this.Container.Resolve<CreativeService>().DisableTripleTap();
        }
    }
}
#endif