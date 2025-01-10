#if GDK_VCONTAINER
namespace TheOneStudio.UITemplate.UITemplate.Scripts.ThirdPartyServices.CollapsibleMREC
{
    using GameFoundation.DI;
    using GameFoundation.Scripts.AssetLibrary;
    using UnityEngine;
    using VContainer;
    using VContainer.Unity;

    public static class CollapsibleMRECVContainer
    {
        private static CollapsibleMRECAd collapsibleMRECAd;
        
        public static void RegisterCollapsibleMREC(this IContainerBuilder builder, Transform parent)
        {
            builder.RegisterComponentInNewPrefabResource<CollapsibleMRECAd>(nameof(CollapsibleMRECAd), Lifetime.Singleton).UnderTransform(parent);
            builder.AutoResolve<CollapsibleMRECAd>();
        }
    }
}
#endif