namespace TheOneStudio.UITemplate.UITemplate.Installers
{
    using Core.AnalyticServices;
    using Core.AnalyticServices.Data;
    using ServiceImplementation.AdsServices;
    using ServiceImplementation.FirebaseAnalyticTracker;
    using TheOneStudio.UITemplate.UITemplate.Scripts.Interfaces;
    using TheOneStudio.UITemplate.UITemplate.Scripts.Services;
    using TheOneStudio.UITemplate.UITemplate.ThirdPartyServices.AnalyticEvents;
    using Zenject;
#if FIREBASE_REMOTE_CONFIG
    using Firebase.RemoteConfig;
#endif
#if APPSFLYER
    using ServiceImplementation.AppsflyerAnalyticTracker;
#endif

    public class UITemplateThirdPartyInstaller : Installer<UITemplateThirdPartyInstaller>
    {
        public override void InstallBindings()
        {
            //Third party service
            AdServiceInstaller.Install(this.Container);
            AnalyticServicesInstaller.Install(this.Container);

#if FIREBASE_REMOTE_CONFIG
            this.Container.BindInterfacesAndSelfTo<FirebaseRemoteConfig>().AsCached().NonLazy();

#else
            this.Container.Bind<IFirebaseRemoteConfig>().To<FirebaseDummyManager>().AsCached().NonLazy();
#endif

            var listFactory     = this.Container.ResolveAll<IAnalyticEventFactory>();
            var analyticFactory = listFactory[0];

            if (listFactory is { Count: > 0 })
            {
                this.Container.Bind<AnalyticsEventCustomizationConfig>().FromInstance(analyticFactory.FireBaseAnalyticsEventCustomizationConfig).WhenInjectedInto<FirebaseAnalyticTracker>();
            }
#if APPSFLYER
            if (listFactory is { Count: > 0 })
            {
                this.Container.Bind<AnalyticsEventCustomizationConfig>().FromInstance(analyticFactory.AppsFlyerAnalyticsEventCustomizationConfig).WhenInjectedInto<AppsflyerTracker>();
            }
#endif
        }
    }
}