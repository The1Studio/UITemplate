namespace TheOneStudio.UITemplate.UITemplate.Scripts.Services.Highlight
{
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
            this.Container.Bind<HighlightController>().FromComponentInNewPrefab(this.highlightController).AsCached();
        }
    }
}