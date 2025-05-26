#if GDK_VCONTAINER
namespace Core.AnalyticServices.Data
{
    using GameFoundation.DI;
    using VContainer;

    public static class SessionWatcherVContainer
    {
        public static void RegisterSessionWatcher(this IContainerBuilder builder)
        {
            builder.Register<SessionWatcher>(Lifetime.Singleton); 
            builder.AutoResolve<SessionWatcher>();
        }
    }
}
#endif