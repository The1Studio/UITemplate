namespace TheOneStudio.UITemplate.UITemplate.Installers
{
    using ServiceImplementation.IAPServices;
    using TheOneStudio.UITemplate.UITemplate.FTUE;
    using TheOneStudio.UITemplate.UITemplate.Scenes.Main.Decoration;
    using UnityEngine;
    using Zenject;

    public class UITemplateInstaller : Installer<GameObject, UITemplateInstaller>
    {
        private readonly GameObject soundGroupPrefab;

        public UITemplateInstaller(GameObject soundGroupPrefab) { this.soundGroupPrefab = soundGroupPrefab; }

        public override void InstallBindings()
        {
            UITemplateDeclareSignalInstaller.Install(this.Container);

            UnityIapInstaller.Install(this.Container);
            FTUEInstaller.Install(this.Container);
            UITemplateLocalDataInstaller.Install(this.Container);
            UITemplateServicesInstaller.Install(this.Container, this.soundGroupPrefab);
            UITemplateThirdPartyInstaller.Install(this.Container);
            UITemplateAdsInstaller.Install(this.Container); // this depend on third party service signals

            //Features: decoration, building, starter pack .....
#if UITEMPLATE_DECORATION
            this.Container.BindInterfacesAndSelfTo<UITemplateDecorationManager>().AsCached().NonLazy();
#endif
        }
    }
}