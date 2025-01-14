#if GDK_VCONTAINER
namespace TheOneStudio.UITemplate.UITemplate.Scripts.ThirdPartyServices.CollapsibleMREC
{
    using GameFoundation.DI;
    using TheOneStudio.UITemplate.UITemplate.Services;
    using UnityEngine;
    using VContainer;

    public static class CollapsibleMrecVContainer
    {
        private static CollapsibleMrecView CollapsibleMRECView;

        public static void RegisterCollapsibleMREC(this IContainerBuilder builder, Transform parent)
        {
            builder.Register<CollapsibleMrecService>(Lifetime.Singleton).WithParameter(_ => parent).AsImplementedInterfaces();
        }
    }
}
#endif