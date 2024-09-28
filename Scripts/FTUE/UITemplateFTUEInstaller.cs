#if GDK_ZENJECT
namespace TheOneStudio.UITemplate.UITemplate.FTUE
{
    using GameFoundation.Scripts.Utilities.Extension;
    using TheOneStudio.UITemplate.UITemplate.FTUE.Conditions;
    using TheOneStudio.UITemplate.UITemplate.FTUE.FTUEListen;
    using Zenject;

    public class UITemplateFTUEInstaller : Installer<UITemplateFTUEInstaller>
    {
        public override void InstallBindings()
        {
            this.Container.Bind<IFtueCondition>()
                .To(convention => convention.AllNonAbstractClasses().DerivingFrom<IFtueCondition>())
                .AsSingle()
                .WhenInjectedInto<UITemplateFTUESystem>();

            this.Container.BindInterfacesAndSelfTo<UITemplateFTUESystem>().AsCached();
            this.Container.Bind<UITemplateFTUEController>().AsCached();
            this.Container.Bind<UITemplateFTUEHelper>().AsCached();
            this.Container.BindInterfacesAndSelfToAllTypeDriveFrom<FTUEBaseListen>();
        }
    }
}
#endif