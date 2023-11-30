namespace TheOneStudio.UITemplate.UITemplate.Services.BreakAds
{
    using Zenject;

    public class BreakAdsInstaller : Installer<BreakAdsInstaller>
    {
        public override void InstallBindings() { this.Container.BindInterfacesAndSelfTo<BreakAdsServices>().AsCached().NonLazy(); }
    }
}