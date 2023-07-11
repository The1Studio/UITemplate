namespace TheOneStudio.UITemplate.UITemplate.Installers
{
    using System.Collections.Generic;
    using Core.AdsServices;
    using GameFoundation.Scripts.Utilities.Extension;
    using global::Models;
    using ServiceImplementation.AdsServices;
    using ServiceImplementation.AdsServices.AppLovin;
#if ADMOB || IRONSOURCE
    using ServiceImplementation.AdsServices.EasyMobile;
#endif
    using ServiceImplementation.FireBaseRemoteConfig;
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

        public override void InstallBindings()
        {
            this.Container.BindInterfacesAndSelfTo<UITemplateAnalyticHandler>().AsCached();
#if CREATIVE
            this.Container.Bind<UITemplateAdServiceWrapper>().To<UITemplateAdServiceWrapperCreative>().AsCached();
#else
            this.Container.BindInterfacesAndSelfTo<UITemplateAdServiceWrapper>().AsCached();
#endif

#if ADMOB
            var admobConfig = this.Container.Resolve<GDKConfig>().GetGameConfig<AdmobAOAConfig>();

            var adMobWrapperConfig = new AdModWrapper.Config(admobConfig.ListAoaAppId)
            {
                ADModMRecIds         = new Dictionary<AdViewPosition, string>(),
                NativeAdIds          = admobConfig.ListNativeId,
                AOAOpenAppThreshHold = admobConfig.AdMObAOAOpenAppThreshold
            };

            this.ConfigureFirebaseRemoteConfig();

            var listMRecAndroidAdViewPosition = admobConfig.listMRecAdViewPosition;

            for (var i = admobConfig.ListMRecId.Count - 1; i >= 0; i--)
            {
                adMobWrapperConfig.ADModMRecIds.Add(listMRecAndroidAdViewPosition[i], admobConfig.ListMRecId[i]);
            }

            this.Container.Bind<AdModWrapper.Config>().FromInstance(adMobWrapperConfig).WhenInjectedInto<AdModWrapper>();
#endif

#if APPLOVIN
            var adConfig = this.Container.Resolve<GDKConfig>().GetGameConfig<AdmobAOAConfig>();

            var adViewPositionToId = new Dictionary<AdViewPosition, string>();
            for (var i = adConfig.ListMRecId.Count - 1; i >= 0; i--)
            {
                adViewPositionToId.Add(adConfig.listMRecAdViewPosition[i], adConfig.ListMRecId[i]);
            }

            this.Container.Bind<Dictionary<AdViewPosition, string>>().FromInstance(adViewPositionToId).WhenInjectedInto<AppLovinAdsWrapper>();
#endif

#if CREATIVE && ADMOB
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
                this.Container.Resolve<AdServicesConfig>().MinPauseSecondToShowAoaAd =
                    this.Container.Resolve<IRemoteConfig>().GetRemoteConfigIntValue(MinPauseSecondsToShowAoaRemoteConfigKey, 0);
            }

            this.Container.Resolve<SignalBus>().Subscribe<RemoteConfigInitializeSucceededSignal>(OnFirebaseInitialized);
        }
    }
}