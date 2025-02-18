#if GDK_VCONTAINER
#nullable enable
namespace TheOneStudio.UITemplate
{
    using GameFoundation.DI;
    using GameFoundation.Scripts.Utilities.Extension;
    using TheOneStudio.UITemplate.UITemplate.Scripts.Services;
    using TheOneStudio.UITemplate.UITemplate.Scripts.ThirdPartyServices;
    using TheOneStudio.UITemplate.UITemplate.Services;
    using TheOneStudio.UITemplate.UITemplate.Services.AnalyticHandler;
    using TheOneStudio.UITemplate.UITemplate.Services.AnalyticHandler.HiGame;
    using VContainer;

    public static class UITemplateAdsVContainer
    {
        public static void RegisterUITemplateAdsService(this IContainerBuilder builder)
        {
            #if BRAVESTARS
            builder.Register<BraveStarsAnalyticHandler>(Lifetime.Singleton).As<UITemplateAnalyticHandler>().AsInterfacesAndSelf();
            #elif APERO
            builder.Register<AperoAnalyticHandler>(Lifetime.Singleton).As<UITemplateAnalyticHandler>().AsInterfacesAndSelf();
            #elif HIGAME
            builder.Register<HiGameAnalyticHandler>(Lifetime.Singleton).As<UITemplateAnalyticHandler>().AsInterfacesAndSelf();
            #else
            builder.RegisterFromDerivedType<UITemplateAnalyticHandler>().AsImplementedInterfaces();
            #endif

            #if CREATIVE
            builder.Register<UITemplateAdServiceWrapper, UITemplateAdServiceWrapperCreative>(Lifetime.Singleton);
            #else
            builder.Register<UITemplateAdServiceWrapper>(Lifetime.Singleton).AsInterfacesAndSelf();
            #endif

            #if CREATIVE
            builder.Register<CreativeService>(Lifetime.Singleton).AsInterfacesAndSelf();
            #endif
        }
    }
}
#endif