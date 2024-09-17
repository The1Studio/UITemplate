#if GDK_VCONTAINER
#nullable enable
namespace TheOneStudio.UITemplate
{
    using GameFoundation.DI;
    using GameFoundation.Scripts.Interfaces;
    using GameFoundation.Scripts.Utilities.Extension;
    using TheOneStudio.UITemplate.UITemplate.Models.Controllers;
    using TheOneStudio.UITemplate.UITemplate.UserData;
    using VContainer;

    public static class UITemplateLocalDataVContainer
    {
        public static void RegisterUITemplateLocalData(this IContainerBuilder builder)
        {
            typeof(ILocalData).GetDerivedTypes().ForEach(type => builder.Register(type, Lifetime.Singleton));
            typeof(IUITemplateControllerData).GetDerivedTypes().ForEach(type => builder.Register(type, Lifetime.Singleton).AsInterfacesAndSelf());
            builder.Register<UserDataManager>(Lifetime.Singleton);
        }
    }
}
#endif