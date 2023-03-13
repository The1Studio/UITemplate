namespace TheOneStudio.UITemplate.UITemplate.Installers
{
    using Core.AnalyticServices;
    using GameFoundation.Scripts.Interfaces;
    using GameFoundation.Scripts.Utilities;
    using global::Models;
    using ServiceImplementation.AdsServices;
    using ServiceImplementation.AdsServices.EasyMobile;
    using TheOneStudio.UITemplate.UITemplate.Interfaces;
    using TheOneStudio.UITemplate.UITemplate.Models;
    using TheOneStudio.UITemplate.UITemplate.Models.Controllers;
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

        public UITemplateInstaller(GameObject soundGroupPrefab) { this.soundGroupPrefab = soundGroupPrefab; }

        public override void InstallBindings()
        {
            this.BindLocalData<UITemplateUserLevelData>();
            this.BindLocalData<UITemplateInventoryData>();
            this.BindLocalData<UITemplateUserSettingData>();
            this.BindLocalData<UITemplateDailyRewardData>();
            this.BindLocalData<UITemplateAdsData>();

            this.Container.Bind<IIapSystem>().To<UITemplateIAPSystem>().AsCached().NonLazy();
            //Signal
            this.Container.DeclareSignal<RewardedAdShowedSignal>();
            this.Container.DeclareSignal<UpdateCurrencySignal>();
            this.Container.DeclareSignal<LevelStartedSignal>();
            this.Container.DeclareSignal<LevelEndedSignal>();
            this.Container.DeclareSignal<LevelSkippedSignal>();
            this.Container.DeclareSignal<InterstitialAdShowedSignal>();
            //Third party service
            AdServiceInstaller.Install(this.Container);
            AnalyticServicesInstaller.Install(this.Container);
#if CREATIVE
            this.Container.Bind<UITemplateAdServiceWrapper>().To<UITemplateAdServiceWrapperDummy>().AsCached();
#else
            this.Container.Bind<UITemplateAdServiceWrapper>().AsCached();
#endif
            this.Container.BindInterfacesAndSelfTo<UITemplateAnalyticHandler>().AsCached();
#if ONE_SOFT
            this.Container.Bind<IAnalyticEventFactory>().To<OneSoftAnalyticEventFactory>().AsCached();
#elif WIDO
            this.Container.Bind<IAnalyticEventFactory>().To<WidoAnalyticEventFactory>().AsCached();
#else
            this.Container.Bind<IAnalyticEventFactory>().To<OneSoftAnalyticEventFactory>().AsCached();
#endif
            //Manager
            this.Container.BindInterfacesAndSelfTo<GameSeasonManager>().AsCached().NonLazy();
            //Build-in service
            this.Container.Bind<IInternetService>().To<InternetService>().AsSingle().NonLazy();

            //Data controller
            this.Container.BindInterfacesAndSelfTo<UITemplateDailyRewardController>().AsCached();
            this.Container.BindInterfacesAndSelfTo<UITemplateInventoryDataController>().AsCached();
            this.Container.BindInterfacesAndSelfTo<UITemplateLevelDataController>().AsCached();
            this.Container.BindInterfacesAndSelfTo<UITemplateSettingDataController>().AsCached();
#if EM_ADMOB
            var adMobWrapperConfig = new AdModWrapper.Config(this.Container.Resolve<GDKConfig>().GetGameConfig<AdmobAOAConfig>().listAoaAppId);
            this.Container.Bind<AdModWrapper.Config>().FromInstance(adMobWrapperConfig).WhenInjectedInto<AdModWrapper>();
#endif

#if CREATIVE
            adMobWrapperConfig.IsShowAOAAtOpenApp = false;
            adMobWrapperConfig.OpenAfterResuming = false;
            this.Container.BindInterfacesAndSelfTo<CreativeService>().AsCached().NonLazy();
#endif

            // Master Audio
            this.Container.InstantiatePrefab(this.soundGroupPrefab);
            this.Container.Bind<UITemplateSoundServices>().AsCached();
            //vibration
            this.Container.Bind<IVibrate>().To<UITemPlateVibrateServices>().AsCached();
        }

        private void BindLocalData<TLocalData>() where TLocalData : class, ILocalData, new()
        {
            this.Container.Bind<TLocalData>().FromResolveGetter<HandleLocalDataServices>(services => services.Load<TLocalData>()).AsCached().NonLazy();
        }
    }
}