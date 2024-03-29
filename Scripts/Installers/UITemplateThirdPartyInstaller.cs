namespace TheOneStudio.UITemplate.UITemplate.Installers
{
    using Core.AnalyticServices;
    using Core.AnalyticServices.Data;
    using GameFoundation.Scripts.Utilities.Extension;
    using ServiceImplementation.AdsServices;
    using ServiceImplementation.Configs;
    using ServiceImplementation.FirebaseAnalyticTracker;
    using ServiceImplementation.FireBaseRemoteConfig;
    using TheOneStudio.UITemplate.UITemplate.Configs.GameEvents;
    using TheOneStudio.UITemplate.UITemplate.ThirdPartyServices.AnalyticEvents;
    using UnityEngine;
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
            FirebaseRemoteConfigInstaller.Install(this.Container);
            var thirdPartiesConfig = Resources.Load<ThirdPartiesConfig>(ThirdPartiesConfig.ResourcePath);
            this.Container.Bind<ThirdPartiesConfig>().FromInstance(thirdPartiesConfig).AsSingle();
            
            //Remove config
            var remoteConfigSetting = Resources.Load<RemoteConfigSetting>(RemoteConfigSetting.ResourcePath);
            this.Container.Bind<RemoteConfigSetting>().FromInstance(remoteConfigSetting).AsSingle();
            
            //Game event config
            var gameFeaturesSetting = Resources.Load<GameFeaturesSetting>(GameFeaturesSetting.ResourcePath);
            this.Container.Bind<GameFeaturesSetting>().FromInstance(gameFeaturesSetting).AsSingle();

            this.Container.BindInterfacesAndSelfToAllTypeDriveFrom<BaseAnalyticEventFactory>();
            var listFactory = this.Container.ResolveAll<IAnalyticEventFactory>();

            if (listFactory.Count > 0)
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