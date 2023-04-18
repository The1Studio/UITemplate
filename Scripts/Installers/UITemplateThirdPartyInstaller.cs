namespace TheOneStudio.UITemplate.UITemplate.Installers
{
    using Core.AnalyticServices;
    using Core.AnalyticServices.Data;
    using GameFoundation.Scripts.Utilities.Extension;
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
            this.Container.BindInterfacesTo<UITemplateFirebaseRemoteConfig>().FromNewComponentOnNewGameObject().AsCached().NonLazy();
#else
            this.Container.Bind<IUITemplateRemoteConfig>().To<UITemplateDummyManager>().AsCached().NonLazy();
#endif

            this.Container.BindInterfacesAndSelfToAllTypeDriveFrom<BaseAnalyticEventFactory>();
            var listFactory     = this.Container.ResolveAll<IAnalyticEventFactory>();

            if (listFactory is { Count: > 0 })
            {
                var analyticFactory = listFactory[0];
                this.Container.Bind<AnalyticsEventCustomizationConfig>().FromInstance(analyticFactory.FireBaseAnalyticsEventCustomizationConfig).WhenInjectedInto<FirebaseAnalyticTracker>();
#if APPSFLYER
                this.Container.Bind<AnalyticsEventCustomizationConfig>().FromInstance(analyticFactory.AppsFlyerAnalyticsEventCustomizationConfig).WhenInjectedInto<AppsflyerTracker>();
#endif
            }
        }
    }
}