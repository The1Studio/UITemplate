namespace TheOneStudio.UITemplate.UITemplate.Installers
{
    using Core.AnalyticServices;
    using Core.AnalyticServices.Data;
    using GameFoundation.Scripts.Interfaces;
    using GameFoundation.Scripts.Utilities;
    using global::Models;
    using ServiceImplementation.AdsServices;
    using ServiceImplementation.AdsServices.EasyMobile;
#if APPSFLYER
    using ServiceImplementation.AppsflyerAnalyticTracker;
#endif
    using ServiceImplementation.FirebaseAnalyticTracker;
    using TheOneStudio.UITemplate.UITemplate.Interfaces;
    using TheOneStudio.UITemplate.UITemplate.Models;
    using TheOneStudio.UITemplate.UITemplate.Models.Controllers;
    using TheOneStudio.UITemplate.UITemplate.Scenes.Popups;
    using TheOneStudio.UITemplate.UITemplate.Scripts.Interfaces;
    using TheOneStudio.UITemplate.UITemplate.Scripts.Services;
    using TheOneStudio.UITemplate.UITemplate.Scripts.Signals;
    using TheOneStudio.UITemplate.UITemplate.Scripts.ThirdPartyServices;
    using TheOneStudio.UITemplate.UITemplate.Scripts.ThirdPartyServices.AnalyticEvents.ABI;
    using TheOneStudio.UITemplate.UITemplate.Scripts.ThirdPartyServices.AnalyticEvents.AdOne;
    using TheOneStudio.UITemplate.UITemplate.Scripts.ThirdPartyServices.AnalyticEvents.OneSoft;
    using TheOneStudio.UITemplate.UITemplate.Scripts.ThirdPartyServices.AnalyticEvents.Wido;
    using TheOneStudio.UITemplate.UITemplate.Services;
    using TheOneStudio.UITemplate.UITemplate.Signals;
    using TheOneStudio.UITemplate.UITemplate.ThirdPartyServices.AnalyticEvents;
    using UnityEngine;
    using Zenject;

#if NOTIFICATION_ENABLE
    using NotificationServices = TheOneStudio.UITemplate.UITemplate.Services.NotificationServices;
#endif

    public class UITemplateInstaller : Installer<GameObject, UITemplateInstaller>
    {
        private readonly GameObject soundGroupPrefab;

        public UITemplateInstaller(GameObject soundGroupPrefab) { this.soundGroupPrefab = soundGroupPrefab; }

        public override void InstallBindings()
        {
            this.BindLocalData<UITemplateUserLevelData>();
            this.BindLocalData<UITemplateInventoryData>();
            this.BindLocalData<UITemplateUserSettingData>();
            this.BindLocalData<UITemplateDailyRewardData>();
            this.BindLocalData<UITemplateUserJackpotData>();
            this.BindLocalData<UITemplateAdsData>();

            this.Container.Bind<UITemplateAdServiceConfig>().AsCached().NonLazy();

#if !TEMPLATE_IAP
            this.Container.Bind<IIapServices>().To<UITemplateDummyIAPServices>().AsCached().NonLazy();
#else
            this.Container.Bind<IIapServices>().To<UITemplateIapServices>().AsCached().NonLazy();
#endif
            //Signal
            this.Container.DeclareSignal<RewardedAdEligibleSignal>();
            this.Container.DeclareSignal<RewardedAdCalledSignal>();
            this.Container.DeclareSignal<RewardedAdOfferSignal>();
            this.Container.DeclareSignal<UpdateCurrencySignal>();
            this.Container.DeclareSignal<TutorialCompletionSignal>();
            this.Container.DeclareSignal<LevelStartedSignal>();
            this.Container.DeclareSignal<LevelEndedSignal>();
            this.Container.DeclareSignal<LevelSkippedSignal>();
            this.Container.DeclareSignal<InterstitialAdCalledSignal>();
            this.Container.DeclareSignal<InterstitialAdEligibleSignal>();
            this.Container.DeclareSignal<FirebaseInitializeSucceededSignal>();
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
            this.Container.Bind<IAnalyticEventFactory>().To<ABIAnalyticEventFactory>().AsCached();
#elif ADONE
            this.Container.Bind<IAnalyticEventFactory>().To<AdOneAnalyticEventFactory>().AsCached();
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
            this.Container.BindInterfacesAndSelfTo<UITemplateJackpotController>().AsCached();
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
#elif UNITY_ANDROID && !UNITY_EDITOR
            this.Container.Bind<IFlashLight>().To<FlashlightPluginAndroid>().AsSingle().NonLazy();
#elif UNITY_IOS && !UNITY_EDITOR
            this.Container.Bind<IFlashLight>().To<FlashLightPluginIOS>().AsSingle().NonLazy();
#endif

#if FIREBASE_REMOTE_CONFIG
            this.Container.Bind<IFirebaseRemoteConfig>().To<FirebaseRemoteConfig>().AsCached().NonLazy();

#else
            this.Container.Bind<IFirebaseRemoteConfig>().To<FirebaseDummyManager>().AsCached().NonLazy();
#endif

#if APPSFLYER
            var analyticFactory = this.Container.Resolve<IAnalyticEventFactory>();
            this.Container.Bind<AnalyticsEventCustomizationConfig>().FromInstance(analyticFactory.AppsFlyerAnalyticsEventCustomizationConfig).WhenInjectedInto<AppsflyerTracker>();
            this.Container.Bind<AnalyticsEventCustomizationConfig>().FromInstance(analyticFactory.FireBaseAnalyticsEventCustomizationConfig).WhenInjectedInto<FirebaseAnalyticTracker>();
#endif

            //Daily Reward
            this.Container.Bind<UITemplateDailyRewardService>().AsCached();

#if NOTIFICATION_ENABLE
            this.Container.BindInterfacesAndSelfTo<NotificationServices>().AsCached().NonLazy();
#endif
        }

        private void BindLocalData<TLocalData>() where TLocalData : class, ILocalData, new()
        {
            this.Container.Bind<TLocalData>().FromResolveGetter<HandleLocalDataServices>(services => services.Load<TLocalData>()).AsCached().NonLazy();
        }
    }
}