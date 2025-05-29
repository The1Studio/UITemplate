#if GDK_VCONTAINER
namespace Core.AnalyticServices.Data
{
    using VContainer;

    public static class SessionWatcherVContainer
    {
        public static void RegisterSessionWatcher(this IContainerBuilder builder)
        {
            builder.Register<SessionWatcher>(Lifetime.Singleton).AsImplementedInterfaces().AsSelf();
            builder.AutoResolve<SessionWatcher>();
        }
    }
}
#endif