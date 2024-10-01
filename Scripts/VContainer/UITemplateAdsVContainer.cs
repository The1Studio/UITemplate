#if GDK_VCONTAINER
#nullable enable
namespace TheOneStudio.UITemplate
{
    using GameFoundation.DI;
    using TheOneStudio.UITemplate.UITemplate.Scripts.ThirdPartyServices;
    using TheOneStudio.UITemplate.UITemplate.Services.AnalyticHandler;
    using VContainer;

    public static class UITemplateAdsVContainer
    {
        public static void RegisterUITemplateAdsService(this IContainerBuilder builder)
        {
            // analytic handler
            #if BRAVESTARS
                builder.Register<BraveStarsAnalyticHandler>(Lifetime.Singleton).AsInterfacesAndSelf();
            #else
                builder.Register<UITemplateAnalyticHandler>(Lifetime.Singleton).AsInterfacesAndSelf();
            #endif
            
            // ad service
            #if CREATIVE
            builder.Register<UITemplateAdServiceWrapper, UITemplateAdServiceWrapperCreative>(Lifetime.Singleton);
            #else
            builder.Register<UITemplateAdServiceWrapper>(Lifetime.Singleton).AsInterfacesAndSelf();
            #endif
        }
    }
}
#endif