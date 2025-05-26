#if GDK_VCONTAINER
namespace Core.AnalyticServices.Data
{
    using GameFoundation.Scripts.UIModule.ScreenFlow.Managers;
    using TheOneStudio.UITemplate.UITemplate.Models.Controllers;
    using VContainer;
    using VContainer.Unity;

    public static class SessionWatcherVContainer
    {
        public static void RegisterSessionWatcher(this IContainerBuilder builder)
        {
            builder.RegisterComponentOnNewGameObject<SessionWatcher>(Lifetime.Singleton);
            builder.RegisterBuildCallback(container => container.Resolve<SessionWatcher>().Construct(container.Resolve<IAnalyticServices>(), container.Resolve<IScreenManager>(), container.Resolve<UITemplateGameSessionDataController>()));
        }
    }
}
#endif