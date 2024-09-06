#if GDK_ZENJECT
namespace TheOneStudio.UITemplate.UITemplate.Installers
{
    using GameFoundation.Scripts.Utilities.Extension;
    using TheOneStudio.UITemplate.UITemplate.Helpers;
    using TheOneStudio.UITemplate.UITemplate.Interfaces;
    using TheOneStudio.UITemplate.UITemplate.Scenes.FeaturesConfig;
    using TheOneStudio.UITemplate.UITemplate.Scripts.Services;
    using TheOneStudio.UITemplate.UITemplate.Services;
    using TheOneStudio.UITemplate.UITemplate.Services.RewardHandle;
    using TheOneStudio.UITemplate.UITemplate.Services.RewardHandle.AllRewards;
    using TheOneStudio.UITemplate.UITemplate.Services.Toast;
    using TheOneStudio.UITemplate.UITemplate.Services.Vibration;
    using TheOneStudio.UITemplate.UITemplate.Utils;
    using Zenject;

    public class UITemplateServicesInstaller : Installer<ToastController, UITemplateServicesInstaller>
    {
        private readonly ToastController toastController;

        public UITemplateServicesInstaller(ToastController toastController)
        {
            this.toastController  = toastController;
        }

        public override void InstallBindings()
        {
            this.Container.BindInterfacesAndSelfTo<UITemplateFeatureConfig>().AsCached().NonLazy();
            // Master Audio
            this.Container.Bind<UITemplateSoundServices>().AsCached();
            //Build-in service
            this.Container.BindInterfacesAndSelfTo<InternetService>().AsSingle().NonLazy();
            //HandleScreenShow
            this.Container.BindInterfacesAndSelfTo<UITemplateScreenShowServices>().AsCached();
            this.Container.BindInterfacesAndSelfToAllTypeDriveFrom<UITemplateBaseScreenShow>();
            //FlyingAnimation Currency
            this.Container.Bind<UITemplateFlyingAnimationController>().AsCached().NonLazy();
            //Utils
            this.Container.Bind<GameAssetUtil>().AsCached();
            //Vibration
            this.Container.Bind<IVibrationService>().To<UITemplateVibrationService>().AsCached();

            this.Container.Bind<UITemplateHandleSoundWhenOpenAdsServices>().AsCached().NonLazy();
            //Reward Handle
            this.Container.BindInterfacesAndSelfToAllTypeDriveFrom<IUITemplateRewardExecutor>();
            this.Container.BindInterfacesAndSelfTo<UITemplateRewardHandler>().AsCached().NonLazy();

            // this.Container.BindInterfacesTo<UITemplateAutoOpenStartedPackServices>().AsCached().NonLazy();

            // VFX Spawn
            this.Container.Bind<UITemplateVFXSpawnService>().AsCached().NonLazy();

            // Toast
            this.Container.Bind<ToastController>().FromComponentInNewPrefab(this.toastController).AsCached().NonLazy();

            //Button experience helper
            this.Container.BindInterfacesAndSelfTo<UITemplateButtonExperienceHelper>().AsSingle().NonLazy();
        }
    }
}
#endif