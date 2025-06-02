#if GDK_VCONTAINER
#nullable enable
namespace TheOneStudio.UITemplate.UITemplate.Creative.Cheat
{
    #if CREATIVE
    using TheOneStudio.UITemplate.UITemplate.Scripts.Services;
    #endif
    using VContainer;

    public static class NewCreativeServiceVContainer
    {
        public static void RegisterNewCreativeService(this IContainerBuilder builder)
        {
            #if CREATIVE
            builder.Register<NewCreativeService>(Lifetime.Singleton).AsImplementedInterfaces().AsSelf();
            builder.RegisterBuildCallback(container => container.Resolve<CreativeService>().DisableTripleTap());
            #endif
        }
    }
}
#endif