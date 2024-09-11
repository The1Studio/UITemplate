#if GDK_VCONTAINER
#nullable enable
namespace TheOneStudio.UITemplate
{
    using System.Linq;
    using Core.AnalyticServices;
    using GameFoundation.Scripts.Utilities.Extension;
    using ServiceImplementation.AdsServices;
    using ServiceImplementation.Configs;
    using ServiceImplementation.FirebaseAnalyticTracker;
    using ServiceImplementation.FireBaseRemoteConfig;
    using ServiceImplementation.IAPServices;
    using ServiceImplementation.RemoteConfig;
    using TheOneStudio.UITemplate.UITemplate.Configs.GameEvents;
    using TheOneStudio.UITemplate.UITemplate.ThirdPartyServices.AnalyticEvents;
    using UnityEngine;
    using VContainer;
    #if APPSFLYER
    using ServiceImplementation.AppsflyerAnalyticTracker;
    #endif
    #if BYTEBREW && !UNITY_EDITOR
    using ServiceImplementation.ByteBrewAnalyticTracker;
    #endif

    public static class UITemplateThirdPartyVContainer
    {
        public static void RegisterUITemplateThirdPartyServices(this IContainerBuilder builder)
        {
            builder.RegisterAdService();
            builder.RegisterIAPService();
            builder.RegisterAnalyticService();
            builder.RegisterRemoteConfig();

            builder.RegisterInstance(Resources.Load<ThirdPartiesConfig>(ThirdPartiesConfig.ResourcePath));
            builder.RegisterInstance(Resources.Load<RemoteConfigSetting>(RemoteConfigSetting.ResourcePath));
            builder.RegisterInstance(Resources.Load<GameFeaturesSetting>(GameFeaturesSetting.ResourcePath));

            builder.Register(typeof(IAnalyticEventFactory).GetDerivedTypes().Single(), Lifetime.Singleton).AsImplementedInterfaces();

            builder.Register<FirebaseAnalyticTracker>(Lifetime.Singleton).AsImplementedInterfaces().WithParameter(container => container.Resolve<IAnalyticEventFactory>().FireBaseAnalyticsEventCustomizationConfig);
            #if APPSFLYER
            builder.Register<AppsflyerTracker>(Lifetime.Singleton).AsImplementedInterfaces().WithParameter(container => container.Resolve<IAnalyticEventFactory>().AppsFlyerAnalyticsEventCustomizationConfig);
            #endif
            #if BYTEBREW && !UNITY_EDITOR
            builder.Register<ByteBrewTracker>(Lifetime.Singleton).AsImplementedInterfaces().WithParameter(container => container.Resolve<IAnalyticEventFactory>().ByteBrewAnalyticsEventCustomizationConfig);
            #endif
        }
    }
}
#endif