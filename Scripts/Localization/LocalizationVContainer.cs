namespace TheOneStudio.UITemplate.UITemplate.Localization
{
    using GameFoundation.DI;
    using GameFoundation.Signals;
    using TheOneStudio.UITemplate.UITemplate.Localization.Signals;
    using VContainer;

    /// <summary>
    /// VContainer registration for localization services
    /// </summary>
    public static class LocalizationVContainer
    {
        public static void RegisterLocalization(this IContainerBuilder builder)
        {
            builder.Register<StringTableLocalizationProvider>(Lifetime.Singleton).AsSelf();

            builder.Register<BlueprintLocalizationService>(Lifetime.Singleton).AsImplementedInterfaces().AsSelf();
            builder.Register<LocalizationManager>(Lifetime.Singleton).AsImplementedInterfaces().AsSelf();

            builder.DeclareSignal<LanguageChangedSignal>();
        }
    }
}