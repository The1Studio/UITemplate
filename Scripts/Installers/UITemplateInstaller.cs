namespace TheOneStudio.UITemplate.UITemplate.Installers
{
    using Core.AnalyticServices;
    using Core.AnalyticServices.Data;
    using GameFoundation.Scripts.Interfaces;
    using GameFoundation.Scripts.Utilities;
    using GameFoundation.Scripts.Utilities.Extension;
    using global::Models;
    using ServiceImplementation.AdsServices;
    using ServiceImplementation.AdsServices.EasyMobile;
#if APPSFLYER
    using ServiceImplementation.AppsflyerAnalyticTracker;
#endif
    using ServiceImplementation.FirebaseAnalyticTracker;
    using TheOneStudio.UITemplate.UITemplate.Extension;
    using TheOneStudio.UITemplate.UITemplate.FTUE;
    using TheOneStudio.UITemplate.UITemplate.FTUE.TutorialTriggerCondition;
    using TheOneStudio.UITemplate.UITemplate.Interfaces;
    using TheOneStudio.UITemplate.UITemplate.Models;
    using TheOneStudio.UITemplate.UITemplate.Models.Controllers;
    using TheOneStudio.UITemplate.UITemplate.Models.LocalDatas;
    using TheOneStudio.UITemplate.UITemplate.Scenes.Popups;
    using TheOneStudio.UITemplate.UITemplate.Scripts.Interfaces;
    using TheOneStudio.UITemplate.UITemplate.Scripts.Services;
    using TheOneStudio.UITemplate.UITemplate.Scripts.Signals;
    using TheOneStudio.UITemplate.UITemplate.Scripts.ThirdPartyServices;
    using TheOneStudio.UITemplate.UITemplate.Services;
    using TheOneStudio.UITemplate.UITemplate.Signals;
    using TheOneStudio.UITemplate.UITemplate.ThirdPartyServices.AnalyticEvents;
    using TheOneStudio.UITemplate.UITemplate.Utils;
    using UnityEngine;
    using Zenject;

    public class UITemplateInstaller : Installer<GameObject, UITemplateInstaller>
    {
        private readonly GameObject soundGroupPrefab;

        public UITemplateInstaller(GameObject soundGroupPrefab) { this.soundGroupPrefab = soundGroupPrefab; }

        public override void InstallBindings()
        {
            FTUEInstaller.Install(this.Container);
            this.Container.BindLocalData<UITemplateUserLevelData>();
            this.Container.BindLocalData<UITemplateInventoryData>();
            this.Container.BindLocalData<UITemplateUserSettingData>();
            this.Container.BindLocalData<UITemplateDailyRewardData>();
            this.Container.BindLocalData<UITemplateUserJackpotData>();
            this.Container.BindLocalData<UITemplateAdsData>();
            this.Container.BindLocalData<UITemplateLuckySpinData>();

            this.Container.BindInterfacesAndSelfToAllTypeDriveFrom<UITemplateBaseFTUE>();
            //HandleScreenShow
            this.Container.BindInterfacesAndSelfTo<UITemplateScreenShowServices>().AsCached();
            this.Container.BindInterfacesAndSelfToAllTypeDriveFrom<UITemplateBaseScreenShow>();
            //AdsConfig
            this.Container.Bind<UITemplateAdServiceConfig>().AsCached().NonLazy();
            //FlyingAnimation Currency
            this.Container.Bind<UITemplateFlyingAnimationCurrency>().AsCached().NonLazy();
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

            //Utils
            this.Container.Bind<GameAssetUtil>().AsCached();

            //Third party service
            AdServiceInstaller.Install(this.Container);
            AnalyticServicesInstaller.Install(this.Container);
#if CREATIVE
            this.Container.Bind<UITemplateAdServiceWrapper>().To<UITemplateAdServiceWrapperCreative>().AsCached();
#else
            this.Container.Bind<UITemplateAdServiceWrapper>().AsCached();
#endif
            this.Container.BindInterfacesAndSelfTo<UITemplateAnalyticHandler>().AsCached();
            this.Container.BindInterfacesAndSelfToAllTypeDriveFrom<BaseAnalyticEventFactory>();

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
            this.Container.BindInterfacesAndSelfTo<UITemplateLuckySpinController>().AsCached();
#if EM_ADMOB
            var listAoaAppId = this.Container.Resolve<GDKConfig>().GetGameConfig<AdmobAOAConfig>().listAoaAppId;
#if UNITY_IOS
            listAoaAppId = this.Container.Resolve<GDKConfig>().GetGameConfig<AdmobAOAConfig>().listAoaIOSAppId;
#endif
            var adMobWrapperConfig = new AdModWrapper.Config(listAoaAppId);
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
            this.Container.BindInterfacesAndSelfTo<FirebaseRemoteConfig>().AsCached().NonLazy();

#else
            this.Container.Bind<IFirebaseRemoteConfig>().To<FirebaseDummyManager>().AsCached().NonLazy();
#endif

#if APPSFLYER
            var listFactory = this.Container.ResolveAll<IAnalyticEventFactory>();

            if (listFactory is { Count: > 0 })
            {
                var analyticFactory = listFactory[0];
                this.Container.Bind<AnalyticsEventCustomizationConfig>().FromInstance(analyticFactory.AppsFlyerAnalyticsEventCustomizationConfig).WhenInjectedInto<AppsflyerTracker>();
                this.Container.Bind<AnalyticsEventCustomizationConfig>().FromInstance(analyticFactory.FireBaseAnalyticsEventCustomizationConfig).WhenInjectedInto<FirebaseAnalyticTracker>();
            }
#endif
        }
    }
}