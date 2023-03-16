namespace TheOneStudio.UITemplate.UITemplate.Installers
{
    using Core.AnalyticServices;
    using GameFoundation.Scripts.Interfaces;
    using GameFoundation.Scripts.Utilities;
    using ServiceImplementation.AdsServices;
    using TheOneStudio.UITemplate.UITemplate.Interfaces;
    using TheOneStudio.UITemplate.UITemplate.Models;
    using TheOneStudio.UITemplate.UITemplate.Models.Controllers;
    using TheOneStudio.UITemplate.UITemplate.Scripts.Interfaces;
    using TheOneStudio.UITemplate.UITemplate.Scripts.Services;
    using TheOneStudio.UITemplate.UITemplate.Scripts.ThirdPartyServices;
    using TheOneStudio.UITemplate.UITemplate.Services;
    using TheOneStudio.UITemplate.UITemplate.Signals;
    using TheOneStudio.UITemplate.UITemplate.ThirdPartyServices.AnalyticEvents;
    using TheOneStudio.UITemplate.UITemplate.ThirdPartyServices.AnalyticEvents.OneSoft;
    using UnityEngine;
    using Zenject;

    public class UITemplateInstaller : Installer<GameObject, UITemplateInstaller>
    {
        private readonly GameObject soundGroupPrefab;

        public UITemplateInstaller(GameObject soundGroupPrefab)
        {
            this.soundGroupPrefab = soundGroupPrefab;
        }

        public override void InstallBindings()
        {
            this.BindLocalData<UITemplateUserLevelData>();
            this.BindLocalData<UITemplateInventoryData>();
            this.BindLocalData<UITemplateUserSettingData>();
            this.BindLocalData<UITemplateDailyRewardData>();
            this.BindLocalData<UITemplateAdsData>();

            this.Container.Bind<UITemplateAdServiceConfig>().AsCached().NonLazy();

#if !TEMPLATE_IAP
            this.Container.Bind<IIapServices>().To<UITemplateDummyIAPServices>().AsCached().NonLazy();
#else
            this.Container.Bind<IIapServices>().To<UITemplateIapServices>().AsCached().NonLazy();
#endif
            //Signal
            this.Container.DeclareSignal<RewardedAdShowedSignal>();
            this.Container.DeclareSignal<RewardedAdClickedSignal>();
            this.Container.DeclareSignal<RewardedAdFailedSignal>();
            this.Container.DeclareSignal<RewardedAdOfferSignal>();
            this.Container.DeclareSignal<UpdateCurrencySignal>();
            this.Container.DeclareSignal<LevelStartedSignal>();
            this.Container.DeclareSignal<LevelEndedSignal>();
            this.Container.DeclareSignal<LevelSkippedSignal>();
            this.Container.DeclareSignal<InterstitialAdShowedSignal>();
            this.Container.DeclareSignal<InterstitialAdClickedSignal>();
            this.Container.DeclareSignal<InterstitialAdFailedSignal>();
            this.Container.DeclareSignal<InterstitialAdLoadedSignal>();
            //Third party service
            AdServiceInstaller.Install(this.Container);
            AnalyticServicesInstaller.Install(this.Container);
#if CREATIVE
            this.Container.Bind<UITemplateAdServiceWrapper>().To<UITemplateAdServiceWrapperCreative>().AsCached();
#else
            this.Container.Bind<UITemplateAdServiceWrapper>().AsCached();
#endif
            this.Container.BindInterfacesAndSelfTo<UITemplateAnalyticHandler>().AsCached();
#if ONESOFT
            this.Container.Bind<IAnalyticEventFactory>().To<OneSoftAnalyticEventFactory>().AsCached();
#elif WIDO
            this.Container.Bind<IAnalyticEventFactory>().To<WidoAnalyticEventFactory>().AsCached();
#elif ABI
            this.Container.Bind<IAnalyticEventFactory>().To<ABIFirebaseAnalyticEventFactory>().AsCached();
            this.Container.Bind<IAnalyticEventFactory>().To<ABIAppsflyerAnalyticEventFactory>().AsCached();
#else
            this.Container.Bind<IAnalyticEventFactory>().To<OneSoftAnalyticEventFactory>().AsCached();
#endif

            //Manager
            this.Container.BindInterfacesAndSelfTo<GameSeasonManager>().AsCached().NonLazy();
            //Build-in service
            this.Container.BindInterfacesAndSelfTo<InternetService>().AsSingle().NonLazy();

            //Data controller
            this.Container.BindInterfacesAndSelfTo<UITemplateDailyRewardController>().AsCached();
            this.Container.BindInterfacesAndSelfTo<UITemplateInventoryDataController>().AsCached();
            this.Container.BindInterfacesAndSelfTo<UITemplateLevelDataController>().AsCached();
            this.Container.BindInterfacesAndSelfTo<UITemplateSettingDataController>().AsCached();
#if EM_ADMOB
            var adMobWrapperConfig = new AdModWrapper.Config(this.Container.Resolve<GDKConfig>().GetGameConfig<AdmobAOAConfig>().listAoaAppId);
            this.Container.Bind<AdModWrapper.Config>().FromInstance(adMobWrapperConfig).WhenInjectedInto<AdModWrapper>();
#endif

#if CREATIVE && EM_ADMOB
            adMobWrapperConfig.IsShowAOAAtOpenApp = false;
            adMobWrapperConfig.OpenAfterResuming = false;
#endif

#if CREATIVE
            this.Container.BindInterfacesAndSelfTo<CreativeService>().AsCached().NonLazy();
#endif
            // Master Audio
            this.Container.InstantiatePrefab(this.soundGroupPrefab);
            this.Container.Bind<UITemplateSoundServices>().AsCached();
            //Vibration
            this.Container.Bind<IVibrate>().To<UITemPlateVibrateServices>().AsCached();
            //FlashLight
#if UNITY_EDITOR
            this.Container.Bind<IFlashLight>().To<FlashLightEditor>().AsSingle().NonLazy();
#endif

#if UNITY_ANDROID && !UNITY_EDITOR
            this.Container.Bind<IFlashLight>().To<FlashlightPluginAndroid>().AsSingle().NonLazy();
#endif

#if UNITY_IOS
        this.Container.Bind<IFlashLight>().To<FlashlightPluginIOS>().AsSingle().NonLazy();
#endif

#if FIREBASE_REMOTE_CONFIG
            this.Container.Bind<IFirebaseRemoteConfig>().To<FirebaseRemoteConfig>().AsCached().NonLazy();

#else
            this.Container.Bind<IFirebaseRemoteConfig>().To<FirebaseDummyManager>().AsCached().NonLazy();
#endif
        }

        private void BindLocalData<TLocalData>() where TLocalData : class, ILocalData, new()
        {
            this.Container.Bind<TLocalData>().FromResolveGetter<HandleLocalDataServices>(services => services.Load<TLocalData>()).AsCached().NonLazy();
        }
    }
}