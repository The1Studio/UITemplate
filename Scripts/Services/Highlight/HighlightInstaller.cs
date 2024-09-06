#if GDK_ZENJECT
namespace TheOneStudio.UITemplate.UITemplate.Scripts.Services.Highlight
{
    using GameFoundation.Scripts.UIModule.ScreenFlow.Managers;
    using GameFoundation.Signals;
    using Zenject;

    public class HighlightInstaller : Installer<HighlightController, HighlightInstaller>
    {
        private readonly HighlightController highlightController;

        public HighlightInstaller(HighlightController highlightController)
        {
            this.highlightController = highlightController;
        }

        public override void InstallBindings()
        {
            this.Container.Bind<HighlightController>()
                .FromComponentInNewPrefab(this.highlightController)
                .AsSingle()
                .OnInstantiated<HighlightController>((ctx, svc) => svc.Construct(ctx.Container.Resolve<IScreenManager>(), ctx.Container.Resolve<SignalBus>()));
        }
    }
}
#endif