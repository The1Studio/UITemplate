namespace TheOneStudio.UITemplate.UITemplate.Installers
{
    using Core.AnalyticServices;
    using ServiceImplementation.AdsServices;
    using TheOneStudio.UITemplate.UITemplate.Scripts.Interfaces;
    using TheOneStudio.UITemplate.UITemplate.Scripts.Services;
    using Zenject;
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