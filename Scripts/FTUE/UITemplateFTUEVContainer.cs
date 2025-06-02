#if GDK_VCONTAINER
#nullable enable
namespace TheOneStudio.UITemplate.UITemplate.FTUE
{
    using System.Linq;
    using TheOne.Extensions;
    using TheOneStudio.UITemplate.UITemplate.FTUE.Conditions;
    using TheOneStudio.UITemplate.UITemplate.FTUE.FTUEListen;
    using TheOneStudio.UITemplate.UITemplate.FTUE.RemoteConfig;
    using TheOneStudio.UITemplate.UITemplate.Models.Controllers;
    using VContainer;

    public static class UITemplateFTUEVContainer
    {
        public static void RegisterFTUESystem(this IContainerBuilder builder)
        {
            builder.Register<UITemplateFTUEDataController>(Lifetime.Singleton);
            builder.Register<UITemplateFTUEController>(Lifetime.Singleton);
            builder.Register<UITemplateFTUEHelper>(Lifetime.Singleton);
            builder.Register<UITemplateFTUEBlueprintDataHandler>(Lifetime.Singleton).AsImplementedInterfaces().AsSelf();
            builder.Register<UITemplateFTUERemoteConfig>(Lifetime.Singleton).AsImplementedInterfaces().AsSelf();
            typeof(FTUEBaseListen).GetDerivedTypes().ForEach(type => builder.Register(type, Lifetime.Singleton).AsImplementedInterfaces());

            builder.Register<UITemplateFTUESystem>(Lifetime.Singleton)
                .WithParameter(container => typeof(IFtueCondition).GetDerivedTypes().Select(type => (IFtueCondition)container.Instantiate(type)).ToList())
                .AsImplementedInterfaces().AsSelf();
        }
    }
}
#endif