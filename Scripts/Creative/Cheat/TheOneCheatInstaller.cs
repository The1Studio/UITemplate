#if GDK_ZENJECT
namespace TheOneStudio.UITemplate.UITemplate.Creative.Cheat
{
#if THEONE_CHEAT
    using TheOneStudio.UITemplate.UITemplate.Scripts.Services;
#endif
    using Zenject;

    public class TheOneCheatInstaller : Installer<TheOneCheatInstaller>
    {
        public override void InstallBindings()
        {
#if THEONE_CHEAT
#if CREATIVE
            this.Container.Resolve<CreativeService>().DisableTripleTap();
#endif
            this.Container.BindInterfacesTo<TheOneCheatGenerate>().AsCached().NonLazy();
#endif
        }
    }
}
#endif