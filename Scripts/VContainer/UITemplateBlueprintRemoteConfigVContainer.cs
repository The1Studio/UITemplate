#if GDK_VCONTAINER
namespace TheOneStudio.UITemplate
{
    using GameFoundation.DI;
    using GameFoundation.Scripts.Utilities.Extension;
    using TheOneStudio.UITemplate.UITemplate.Models.Core.Interface;
    using VContainer;

    public static class UITemplateBlueprintRemoteConfigVContainer
    {
        public static void RegisterBlueprintRemoteConfig(this IContainerBuilder builder)
        {
            typeof(IUITemplateBlueprintRemoteConfig).GetDerivedTypes().ForEach(type => builder.Register(type, Lifetime.Singleton).AsInterfacesAndSelf());
            typeof(IUITemplateBlueprintRemoteHandler).GetDerivedTypes().ForEach(type => builder.Register(type, Lifetime.Singleton));
        }
    }
}
#endif