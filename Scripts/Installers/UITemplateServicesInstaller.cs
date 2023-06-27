namespace TheOneStudio.UITemplate.UITemplate.Installers
{

    using GameFoundation.Scripts.Utilities.Extension;
    using TheOneStudio.UITemplate.UITemplate.Scripts.Services;
    using TheOneStudio.UITemplate.UITemplate.Services;
    using TheOneStudio.UITemplate.UITemplate.Services.Toast;
    using UnityEngine;
    using Zenject;

    public class UITemplateServicesInstaller : Installer<ToastController, UITemplateServicesInstaller>
    {
        private readonly ToastController toastController;

        public UITemplateServicesInstaller(ToastController toastController)
        {
            this.toastController = toastController;
        }

        public override void InstallBindings()
        {
            // Master Audio
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

            // Signal Observer
            this.Container.Bind<UITemplateSignalObserver>().AsCached().NonLazy();
        }
    }

}