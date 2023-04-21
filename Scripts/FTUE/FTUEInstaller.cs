namespace TheOneStudio.UITemplate.UITemplate.FTUE
{
    using GameFoundation.Scripts.Utilities.Extension;
    using TheOneStudio.UITemplate.UITemplate.Extension;
    using TheOneStudio.UITemplate.UITemplate.FTUE.FTUEListen;
    using TheOneStudio.UITemplate.UITemplate.Models;
    using TheOneStudio.UITemplate.UITemplate.Models.Controllers;
    using Zenject;

    public class FTUEInstaller : Installer<FTUEInstaller>
    {
        public override void InstallBindings()
        {
            this.Container.BindInterfacesAndSelfTo<UITemplateFTUESystem>().AsCached().NonLazy();
            this.Container.Bind<UITemplateFTUEController>().FromComponentInNewPrefabResource(nameof(UITemplateFTUEController)).AsCached().NonLazy();
            this.Container.Bind<UITemplateFTUEHelper>().AsCached().NonLazy();
            this.Container.BindInterfacesAndSelfToAllTypeDriveFrom<FTUEBaseListen>();
        }
    }
}