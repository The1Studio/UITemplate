namespace TheOneStudio.UITemplate.UITemplate.Installers
{
    using System.Collections.Generic;
    using Core.AnalyticServices;
    using GameFoundation.Scripts.Interfaces;
    using GameFoundation.Scripts.Utilities;
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
    using Zenject;

    public class UITemplateInstaller : Installer<UITemplateInstaller>
    {
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
            this.Container.Bind<UITemplateAdServiceWrapper>().AsCached();
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
        }

        private void BindLocalData<TLocalData>() where TLocalData : class, ILocalData, new()
        {
            this.Container.Bind<TLocalData>().FromResolveGetter<HandleLocalDataServices>(services => services.Load<TLocalData>()).AsCached().NonLazy();
        }

        public static void BindAOAAdConfig(DiContainer container, List<string> aoaId)
        {
#if EM_ADMOD
            var AdMobWrapperConfig = new AdModWrapper.Config(aoaId);
            container.Bind<AdModWrapper.Config>().FromInstance(AdMobWrapperConfig).WhenInjectedInto<AdModWrapper>();
#endif
        }
    }
}