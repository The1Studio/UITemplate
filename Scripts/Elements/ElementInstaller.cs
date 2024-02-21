namespace TheOneStudio.UITemplate.UITemplate.Elements
{
    using Zenject;

    public class ElementInstaller : Installer<ElementInstaller>
    {
        public override void InstallBindings()
        {
            this.Container.Bind<ElementManager>().AsSingle().NonLazy();
        }
    }
}