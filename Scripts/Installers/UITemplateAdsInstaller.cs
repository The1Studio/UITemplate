namespace TheOneStudio.UITemplate.UITemplate.Installers
{
    using TheOneStudio.UITemplate.UITemplate.Scripts.ThirdPartyServices;
    using Zenject;

    public class UITemplateAdsInstaller : Installer<UITemplateAdsInstaller>
    {
        public override void InstallBindings()
        {
            this.Container.BindInterfacesAndSelfTo<UITemplateAdServiceWrapper>().AsCached();
        }
    }
}