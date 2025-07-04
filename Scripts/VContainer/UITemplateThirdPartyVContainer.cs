﻿#if GDK_VCONTAINER
#nullable enable
namespace TheOneStudio.UITemplate
{
    using System.Linq;
    using Core.AnalyticServices;
    using Core.AnalyticServices.Data;
    using ServiceImplementation.AdsServices;
#if THEONE_FACEBOOK_SDK
    using ServiceImplementation.Analytic.FacebookSDK;
#endif
    using ServiceImplementation.Configs;
    using ServiceImplementation.FirebaseAnalyticTracker;
    using ServiceImplementation.FireBaseRemoteConfig;
    using ServiceImplementation.IAPServices;
    using ServiceImplementation.RemoteConfig;
    using TheOne.Extensions;
    using TheOneStudio.UITemplate.UITemplate.Configs.GameEvents;
    using TheOneStudio.UITemplate.UITemplate.ThirdPartyServices.AnalyticEvents;
    using VContainer;
    #if APPSFLYER
    using ServiceImplementation.AppsflyerAnalyticTracker;
    #endif

    #if BYTEBREW && !UNITY_EDITOR
    using ServiceImplementation.ByteBrewAnalyticTracker;
    #endif
    #if ADJUST
    using ServiceImplementation.AdjustAnalyticTracker;
    #endif

    public static class UITemplateThirdPartyVContainer
    {
        public static void RegisterUITemplateThirdPartyServices(this IContainerBuilder builder)
        {
            builder.RegisterAdService();
            builder.RegisterIAPService();
            builder.RegisterAnalyticService();
            builder.RegisterRemoteConfig();

            builder.RegisterResource<ThirdPartiesConfig>(ThirdPartiesConfig.ResourcePath, Lifetime.Singleton);
            builder.RegisterResource<RemoteConfigSetting>(RemoteConfigSetting.ResourcePath, Lifetime.Singleton);
            builder.RegisterResource<GameFeaturesSetting>(GameFeaturesSetting.ResourcePath, Lifetime.Singleton);

            builder.Register(typeof(IAnalyticEventFactory).GetSingleDerivedType(), Lifetime.Singleton).AsImplementedInterfaces();

            builder.Register<FirebaseAnalyticTracker>(Lifetime.Singleton).AsImplementedInterfaces().WithParameter(container => container.Resolve<IAnalyticEventFactory>().FireBaseAnalyticsEventCustomizationConfig);
            #if APPSFLYER
            builder.Register<AppsflyerTracker>(Lifetime.Singleton).AsImplementedInterfaces().WithParameter(container => container.Resolve<IAnalyticEventFactory>().AppsFlyerAnalyticsEventCustomizationConfig);
            #endif
            #if BYTEBREW && !UNITY_EDITOR
            builder.Register<ByteBrewTracker>(Lifetime.Singleton).AsImplementedInterfaces().WithParameter(container => container.Resolve<IAnalyticEventFactory>().ByteBrewAnalyticsEventCustomizationConfig);
            #endif
            #if ADJUST
            builder.Register<AdjustTracker>(Lifetime.Singleton).AsImplementedInterfaces().WithParameter(container => container.ResolveOrDefault<AnalyticsEventCustomizationConfig>(new()));
            #endif

            #if THEONE_FACEBOOK_SDK
            builder.Register<FacebookSDKService>(Lifetime.Singleton).AsInterfacesAndSelf();
            #endif
        }
    }
}
#endif