#if GDK_VCONTAINER
namespace TheOneStudio.UITemplate.UITemplate.Scripts.ThirdPartyServices.CollapsibleMREC
{
    using GameFoundation.Scripts.UIModule.ScreenFlow.Managers;
    using ServiceImplementation.Configs.Ads;
    using VContainer;
    using VContainer.Unity;

    public static class CollapsibleMRECVContainer
    {
        public static void RegisterCollapsibleMREC(this IContainerBuilder builder, CollapsibleMRECAd collapsibleMRECAd)
        {
            builder.RegisterComponentInNewPrefab(collapsibleMRECAd, Lifetime.Singleton);
            builder.RegisterBuildCallback(container =>
                container.Resolve<CollapsibleMRECAd>().Inject(container.Resolve<IScreenManager>(),
                    container.Resolve<UITemplateAdServiceWrapper>(),
                    container.Resolve<AdServicesConfig>()));
        }
    }
}
#endif