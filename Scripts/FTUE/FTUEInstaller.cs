namespace TheOneStudio.UITemplate.UITemplate.FTUE
{
    using TheOneStudio.UITemplate.UITemplate.Extension;
    using TheOneStudio.UITemplate.UITemplate.FTUE.Signal;
    using TheOneStudio.UITemplate.UITemplate.Models;
    using TheOneStudio.UITemplate.UITemplate.Models.Controllers;
    using Zenject;

    public class FTUEInstaller : Installer<FTUEInstaller>
    {
        public override void InstallBindings()
        {
            this.Container.BindLocalData<UITemplateFTUEData>();
            this.Container.BindInterfacesAndSelfTo<UITemplateFTUEControllerData>().AsCached();
            this.Container.BindInterfacesAndSelfTo<UITemplateFTUESystem>().AsCached().NonLazy();
            this.Container.Bind<UITemplateFTUEController>().FromComponentInNewPrefabResource(nameof(UITemplateFTUEController)).AsCached().NonLazy();
            this.Container.Bind<UITemplateFTUEHelper>().AsCached().NonLazy();
            this.Container.DeclareSignal<FTUEButtonClickSignal>();
            this.Container.DeclareSignal<FTUEManualTriggerSignal>();
        }
    }
}