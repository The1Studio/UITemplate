namespace TheOneStudio.UITemplate.UITemplate.Installers
{
    using Core.AnalyticServices;
    using Core.AnalyticServices.Data;
    using ServiceImplementation.AdsServices;
    using ServiceImplementation.FirebaseAnalyticTracker;
    using TheOneStudio.UITemplate.UITemplate.ThirdPartyServices.AnalyticEvents;
    using Zenject;
#if !FIREBASE_REMOTE_CONFIG
    using TheOneStudio.UITemplate.UITemplate.Interfaces;
    using TheOneStudio.UITemplate.UITemplate.Scripts.Services;

#elif FIREBASE_REMOTE_CONFIG
 using TheOneStudio.UITemplate.UITemplate.Services;
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
            this.Container.BindInterfacesTo<UITemplateFirebaseRemoteConfig>().AsCached().NonLazy();
#else
            this.Container.Bind<IUITemplateRemoteConfig>().To<UITemplateDummyManager>().AsCached().NonLazy();
#endif

            var listFactory     = this.Container.ResolveAll<IAnalyticEventFactory>();
            var analyticFactory = listFactory[0];

            if (listFactory is { Count: > 0 })
            {
                this.Container.Bind<AnalyticsEventCustomizationConfig>().FromInstance(analyticFactory.FireBaseAnalyticsEventCustomizationConfig).WhenInjectedInto<FirebaseAnalyticTracker>();
#if APPSFLYER
                this.Container.Bind<AnalyticsEventCustomizationConfig>().FromInstance(analyticFactory.AppsFlyerAnalyticsEventCustomizationConfig).WhenInjectedInto<AppsflyerTracker>();
#endif
            }
        }
    }
}