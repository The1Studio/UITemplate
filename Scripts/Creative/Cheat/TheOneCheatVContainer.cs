#if GDK_VCONTAINER
#nullable enable
namespace TheOneStudio.UITemplate.UITemplate.Creative.Cheat
{
    #if THEONE_CHEAT
    using GameFoundation.DI;
    using TheOneStudio.UITemplate.UITemplate.Scripts.Services;
    #endif
    using UnityEngine;
    using VContainer;

    public static class TheOneCheatVContainer
    {
        public static void RegisterCheatView(this IContainerBuilder builder)
        {
            #if THEONE_CHEAT
            #if CREATIVE
            builder.RegisterBuildCallback(container => container.Resolve<CreativeService>().DisableTripleTap());
            #endif
            builder.Register<TheOneCheatGenerate>(Lifetime.Singleton).AsImplementedInterfaces();
            #endif
        }
    }
}
#endif