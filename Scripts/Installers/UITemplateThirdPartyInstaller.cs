namespace TheOneStudio.UITemplate.UITemplate.Installers
{
    using ServiceImplementation.AdsServices;
    using Zenject;

    public class UITemplateThirdPartyInstaller : Installer<UITemplateThirdPartyInstaller>
    {
        public override void InstallBindings()
        {
            //Third party service
            AdServiceInstaller.Install(this.Container);
        }
    }
}
