#if GDK_VCONTAINER
#nullable enable
namespace TheOneStudio.UITemplate
{
    using GameFoundation.DI;
    using GameFoundation.Scripts.Utilities.Extension;
    using TheOneStudio.UITemplate.UITemplate.Scripts.Services;
    using TheOneStudio.UITemplate.UITemplate.Scripts.ThirdPartyServices;
    using TheOneStudio.UITemplate.UITemplate.Services;
    using VContainer;

    public static class UITemplateAdsVContainer
    {
        public static void RegisterUITemplateAdsService(this IContainerBuilder builder)
        {
            builder.RegisterFromDerivedType<UITemplateAnalyticHandler>().AsImplementedInterfaces();
            builder.RegisterFromDerivedType<UITemplateAdServiceWrapper>().AsImplementedInterfaces();

            #if CREATIVE
            builder.Register<CreativeService>(Lifetime.Singleton).AsInterfacesAndSelf();
            #endif
        }
    }
}
#endif