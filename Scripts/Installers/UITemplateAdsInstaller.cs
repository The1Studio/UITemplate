namespace TheOneStudio.UITemplate.UITemplate.Installers
{
    using System.Collections.Generic;
    using Core.AdsServices;
    using GameFoundation.Scripts.Utilities.Extension;
    using global::Models;
    using ServiceImplementation.AdsServices.EasyMobile;
    using TheOneStudio.UITemplate.UITemplate.Interfaces;
    using TheOneStudio.UITemplate.UITemplate.Scripts.Services;
    using TheOneStudio.UITemplate.UITemplate.Scripts.Signals;
    using TheOneStudio.UITemplate.UITemplate.Scripts.ThirdPartyServices;
    using TheOneStudio.UITemplate.UITemplate.Services;
    using TheOneStudio.UITemplate.UITemplate.ThirdPartyServices.AnalyticEvents;
    using Zenject;

    public class UITemplateAdsInstaller : Installer<UITemplateAdsInstaller>
    {
        private const string MinPauseSecondsToShowAoaRemoteConfigKey = "min_pause_seconds_to_show_aoa";
        
        private AdModWrapper.Config adMobWrapperConfig;
        
        public override void InstallBindings()
        {
            //AdsConfig
            this.Container.Bind<UITemplateAdServiceConfig>().AsCached().NonLazy();
            this.Container.BindInterfacesAndSelfTo<UITemplateAnalyticHandler>().AsCached();
#if CREATIVE
            this.Container.Bind<UITemplateAdServiceWrapper>().To<UITemplateAdServiceWrapperCreative>().AsCached();
#else
            this.Container.BindInterfacesAndSelfTo<UITemplateAdServiceWrapper>().AsCached();
#endif

#if EM_ADMOB
            var admobConfig           = this.Container.Resolve<GDKConfig>().GetGameConfig<AdmobAOAConfig>();
            this.adMobWrapperConfig = new AdModWrapper.Config(admobConfig.ListAoaAppId)
            {
                ADModMRecIds          = new Dictionary<AdViewPosition, string>(),
                NativeAdIds           = admobConfig.ListNativeId,
                AOAOpenAppThreshHold  = admobConfig.AdMObAOAOpenAppThreshold,
                MinPauseTimeToShowAOA = admobConfig.AOAMinPauseTimeToOpen,
            };
            this.ConfigureFirebaseRemoteConfig();

            var listMRecAndroidAdViewPosition = admobConfig.listMRecAdViewPosition;
            for (var i = admobConfig.ListMRecId.Count - 1; i >= 0; i--)
            {
                this.adMobWrapperConfig.ADModMRecIds.Add(listMRecAndroidAdViewPosition[i], admobConfig.ListMRecId[i]);
            }

            this.Container.Bind<AdModWrapper.Config>().FromInstance(this.adMobWrapperConfig).WhenInjectedInto<AdModWrapper>();
#endif

#if CREATIVE && EM_ADMOB
            adMobWrapperConfig.IsShowAOAAtOpenApp = false;
            adMobWrapperConfig.OpenAOAAfterResuming = false;
#endif

#if CREATIVE
            this.Container.BindInterfacesAndSelfTo<CreativeService>().AsCached().NonLazy();
#endif
        }
        
        private void ConfigureFirebaseRemoteConfig()
        {
            void OnFirebaseInitialized()
            {
                //Configure ad service
                this.adMobWrapperConfig.MinPauseTimeToShowAOA = this.Container.Resolve<IUITemplateRemoteConfig>().GetRemoteConfigIntValue(MinPauseSecondsToShowAoaRemoteConfigKey, 0);
            }

            this.Container.Resolve<SignalBus>().Subscribe<RemoteConfigInitializeSucceededSignal>(OnFirebaseInitialized);
        }
    }
}