#if GDK_VCONTAINER
#nullable enable
namespace TheOneStudio.UITemplate.UITemplate.Scripts.Services.Highlight
{
    using GameFoundation.Scripts.UIModule.ScreenFlow.Managers;
    using GameFoundation.Signals;
    using VContainer;
    using VContainer.Unity;

    public static class HighLightControllerVContainer
    {
        public static void RegisterHighlightController(this IContainerBuilder builder, HighlightController highlightControllerPrefab)
        {
            builder.RegisterComponentInNewPrefab(highlightControllerPrefab, Lifetime.Singleton);
            builder.RegisterBuildCallback(container => container.Resolve<HighlightController>().Construct(container.Resolve<IScreenManager>(), container.Resolve<SignalBus>()));
        }
    }
}
#endif