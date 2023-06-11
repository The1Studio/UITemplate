namespace TheOneStudio.UITemplate.UITemplate.Installers
{
    using GameFoundation.Scripts.Utilities.Extension;
    using TheOneStudio.UITemplate.UITemplate.Scripts.Services;
    using TheOneStudio.UITemplate.UITemplate.Services;
    using TheOneStudio.UITemplate.UITemplate.Services.Toast;
    using UnityEngine;
    using Zenject;

    public class UITemplateServicesInstaller : Installer<GameObject, ToastController, UITemplateServicesInstaller>
    {
        private readonly GameObject      soundGroupPrefab;
        private readonly ToastController toastController;

        public UITemplateServicesInstaller(GameObject soundGroupPrefab, ToastController toastController)
        {
            this.soundGroupPrefab = soundGroupPrefab;
            this.toastController  = toastController;
        }

        public override void InstallBindings()
        {
            // Master Audio
            this.Container.InstantiatePrefab(this.soundGroupPrefab);
            this.Container.Bind<UITemplateSoundServices>().AsCached();
            //HandleScreenShow
            this.Container.BindInterfacesAndSelfTo<UITemplateScreenShowServices>().AsCached();
            this.Container.BindInterfacesAndSelfToAllTypeDriveFrom<UITemplateBaseScreenShow>();
            //FlyingAnimation Currency
            this.Container.Bind<UITemplateFlyingAnimationCurrency>().AsCached().NonLazy();
            //Utils
            this.Container.Bind<UITemplateHandleSoundWhenOpenAdsServices>().AsCached().NonLazy();

            // this.Container.BindInterfacesTo<UITemplateAutoOpenStartedPackServices>().AsCached().NonLazy();
            
            // VFX Spawn
            this.Container.Bind<UITemplateVFXSpawnService>().AsCached().NonLazy();
            
            // Toast
            this.Container.Bind<ToastController>().FromComponentInNewPrefab(this.toastController).AsCached().NonLazy();
        }
    }
}