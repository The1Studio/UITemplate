namespace TheOneStudio.UITemplate.UITemplate.Installers
{
    using GameFoundation.Scripts.Utilities.Extension;
    using TheOneStudio.UITemplate.UITemplate.Interfaces;
    using TheOneStudio.UITemplate.UITemplate.Scripts.Services;
    using TheOneStudio.UITemplate.UITemplate.Services;
    using TheOneStudio.UITemplate.UITemplate.Services.RewardHandle;
    using TheOneStudio.UITemplate.UITemplate.Services.RewardHandle.AllRewards;
    using TheOneStudio.UITemplate.UITemplate.Utils;
    using UnityEngine;
    using Zenject;
    using NotificationServices = TheOneStudio.UITemplate.UITemplate.Services.NotificationServices;

    public class UITemplateServicesInstaller : Installer<GameObject, UITemplateServicesInstaller>
    {
        private readonly GameObject soundGroupPrefab;

        public UITemplateServicesInstaller(GameObject soundGroupPrefab) { this.soundGroupPrefab = soundGroupPrefab; }

        public override void InstallBindings()
        {
            //reporter
#if ENABLE_REPORTER
            this.Container.Bind<Reporter>().FromComponentInNewPrefabResource("Reporter").AsSingle().NonLazy();
#endif
            // Master Audio
            this.Container.InstantiatePrefab(this.soundGroupPrefab);
            this.Container.Bind<UITemplateSoundServices>().AsCached();
            
            //Build-in service
            this.Container.BindInterfacesAndSelfTo<InternetService>().AsSingle().NonLazy();
            //HandleScreenShow
            this.Container.BindInterfacesAndSelfTo<UITemplateScreenShowServices>().AsCached();
            this.Container.BindInterfacesAndSelfToAllTypeDriveFrom<UITemplateBaseScreenShow>();
            //FlyingAnimation Currency
            this.Container.Bind<UITemplateFlyingAnimationCurrency>().AsCached().NonLazy();
            //Utils
            this.Container.Bind<GameAssetUtil>().AsCached();
            //Vibration
            this.Container.Bind<IVibrate>().To<UITemPlateVibrateServices>().AsCached();
            //FlashLight
#if UNITY_EDITOR
            this.Container.Bind<IFlashLight>().To<FlashLightEditor>().AsSingle().NonLazy();
#elif UNITY_ANDROID && !UNITY_EDITOR
            this.Container.Bind<IFlashLight>().To<FlashlightPluginAndroid>().AsSingle().NonLazy();
#elif UNITY_IOS && !UNITY_EDITOR
            this.Container.Bind<IFlashLight>().To<FlashLightPluginIOS>().AsSingle().NonLazy();
#endif
            this.Container.Bind<UITemplateHandleSoundWhenOpenAdsServices>().AsCached().NonLazy();
            //Reward Handle
            this.Container.BindInterfacesAndSelfToAllTypeDriveFrom<IUITemplateBaseReward>();
            this.Container.BindInterfacesAndSelfTo<UITemplateHandleRewardServices>().AsCached().NonLazy();
            this.Container.Bind<UITemplateGetRealRewardHelper>().AsCached().NonLazy();

            // this.Container.BindInterfacesTo<UITemplateAutoOpenStartedPackServices>().AsCached().NonLazy();
            
            // VFX Spawn
            this.Container.Bind<UITemplateVFXSpawnService>().AsCached().NonLazy();
        }
    }
}