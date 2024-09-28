#if GDK_VCONTAINER
#nullable enable
namespace TheOneStudio.UITemplate.UITemplate.FTUE
{
    using System.Linq;
    using GameFoundation.DI;
    using GameFoundation.Scripts.Utilities.Extension;
    using TheOneStudio.UITemplate.UITemplate.FTUE.Conditions;
    using TheOneStudio.UITemplate.UITemplate.FTUE.FTUEListen;
    using VContainer;

    public static class UITemplateFTUEVContainer
    {
        public static void RegisterFTUESystem(this IContainerBuilder builder)
        {
            builder.Register<UITemplateFTUEController>(Lifetime.Singleton);
            builder.Register<UITemplateFTUEHelper>(Lifetime.Singleton);
            typeof(FTUEBaseListen).GetDerivedTypes().ForEach(type => builder.Register(type, Lifetime.Singleton).AsImplementedInterfaces());

            builder.Register<UITemplateFTUESystem>(Lifetime.Singleton)
                .WithParameter(container => typeof(IFtueCondition).GetDerivedTypes().Select(type => (IFtueCondition)container.Instantiate(type)).ToList())
                .AsInterfacesAndSelf();
        }
    }
}
#endif