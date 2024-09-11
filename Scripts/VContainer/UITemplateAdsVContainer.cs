#if GDK_VCONTAINER
#nullable enable
namespace TheOneStudio.UITemplate
{
    using TheOneStudio.UITemplate.UITemplate.Scripts.ThirdPartyServices;
    using TheOneStudio.UITemplate.UITemplate.Services;
    using VContainer;

    public static class UITemplateAdsVContainer
    {
        public static void RegisterUITemplateAdsService(this IContainerBuilder builder)
        {
            #if !THEONE_PLAYABLE_ADS
            #if BRAVESTARS
            builder.Register<BraveStarsAnalyticHandler>(Lifetime.Singleton);
            #else
            builder.Register<UITemplateAnalyticHandler>(Lifetime.Singleton);
            #endif
            #endif

            #if CREATIVE
            builder.Register<UITemplateAdServiceWrapper, UITemplateAdServiceWrapperCreative>(Lifetime.Singleton);
            #else
            builder.Register<UITemplateAdServiceWrapper>(Lifetime.Singleton);
            #endif

            #if CREATIVE
            builder.Register<CreativeService>(Lifetime.Singleton);
            #endif
        }
    }
}
#endif