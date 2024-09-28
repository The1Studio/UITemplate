#if GDK_VCONTAINER
#nullable enable
namespace TheOneStudio.UITemplate.UITemplate.Creative.Cheat
{
    using GameFoundation.DI;
    using TheOneStudio.UITemplate.UITemplate.Scripts.Services;
    using VContainer;

    public static class NewCreativeServiceVContainer
    {
        public static void RegisterNewCreativeService(this IContainerBuilder builder)
        {
            #if !CREATIVE
            return;
            #endif
            builder.Register<NewCreativeService>(Lifetime.Singleton).AsInterfacesAndSelf();
            builder.RegisterBuildCallback(container => container.Resolve<CreativeService>().DisableTripleTap());
        }
    }
}
#endif