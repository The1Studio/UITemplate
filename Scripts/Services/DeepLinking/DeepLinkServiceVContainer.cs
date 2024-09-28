#if GDK_VCONTAINER
#nullable enable
namespace TheOneStudio.DeepLinking
{
    using GameFoundation.Signals;
    using TheOneStudio.UITemplate.UITemplate.Services.DeepLinking;
    using VContainer;

    public static class DeepLinkServiceVContainer
    {
        public static void RegisterDeepLinkService(this IContainerBuilder builder)
        {
            builder.Register<DeepLinkService>(Lifetime.Singleton).AsImplementedInterfaces();
            builder.DeclareSignal<OnDeepLinkActiveSignal>();
        }
    }
}
#endif