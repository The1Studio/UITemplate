#if GDK_VCONTAINER
#nullable enable
namespace TheOneStudio.UITemplate
{
    using GameFoundation.DI;
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
            builder.Register<BraveStarsAnalyticHandler>(Lifetime.Singleton).AsInterfacesAndSelf();
            #elif APERO
            builder.Register<AperoAnalyticHandler>(Lifetime.Singleton).AsInterfacesAndSelf();
            #elif HIGAME
            builder.Register<HiGameAnalyticHandler>(Lifetime.Singleton).AsInterfacesAndSelf();
            #else
            builder.Register<UITemplateAnalyticHandler>(Lifetime.Singleton).AsInterfacesAndSelf();
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