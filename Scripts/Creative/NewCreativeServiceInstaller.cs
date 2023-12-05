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
            this.Container.DeclareSignal<ChangeLevelCreativeSignal>();

            this.Container.BindInterfacesAndSelfTo<NewCreativeService>().AsCached().NonLazy();

#if TRIPLE_TAP_CREATIVE
            this.Container.Resolve<CreativeService>().EnableTripleTap = true;
#else
            this.Container.Resolve<CreativeService>().EnableTripleTap = false;
#endif
        }
    }
}