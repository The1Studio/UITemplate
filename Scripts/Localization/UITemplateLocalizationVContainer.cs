#if THEONE_LOCALIZATION && GDK_VCONTAINER
namespace TheOneStudio.UITemplate.Localization
{
    using VContainer;
    public static class UITemplateLocalizationVContainer
    {
        public static void RegisterLocalization(this IContainerBuilder builder)
        {
            // Register core localization services
            builder.Register<BlueprintLocalizationManager>(Lifetime.Singleton)
                   .AsImplementedInterfaces()
                   .AsSelf();

            builder.Register<LanguageService>(Lifetime.Singleton)
                   .AsSelf();

            DeclareLocalizationSignals(builder);
        }

        private static void DeclareLocalizationSignals(IContainerBuilder builder)
        {
            builder.Register<LanguageChangedSignal>(Lifetime.Transient).AsSelf();
        }
    }
}
#endif