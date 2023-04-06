namespace TheOneStudio.UITemplate.UITemplate.Installers
{
    using ServiceImplementation.IAPServices;
    using TheOneStudio.UITemplate.UITemplate.FTUE;
    using UnityEngine;
    using Zenject;

    public class UITemplateInstaller : Installer<GameObject, UITemplateInstaller>
    {
        private readonly GameObject soundGroupPrefab;

        public UITemplateInstaller(GameObject soundGroupPrefab) { this.soundGroupPrefab = soundGroupPrefab; }

        public override void InstallBindings()
        {
            UnityIapInstaller.Install(this.Container);
            FTUEInstaller.Install(this.Container);
            UITemplateAdsInstaller.Install(this.Container);
            UITemplateInitSignalInstaller.Install(this.Container);
            UITemplateLocalDataInstaller.Install(this.Container);
            UITemplateServicesInstaller.Install(this.Container, this.soundGroupPrefab);
            UITemplateThirdPartyInstaller.Install(this.Container);
        }
    }
}