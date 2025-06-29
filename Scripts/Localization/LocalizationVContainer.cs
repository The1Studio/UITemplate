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
        /// <summary>
        /// Register localization services with Unity String Table provider
        /// </summary>
        /// <param name="builder">Container builder</param>
        public static void RegisterLocalization(this IContainerBuilder builder)
        {
            // Register Unity string table localization provider
            builder.Register<UnityStringTableLocalizationProvider>(Lifetime.Singleton);
            
            // Register localization manager (main API)
            builder.Register<LocalizationManager>(Lifetime.Singleton).AsImplementedInterfaces();
            
            // Register blueprint localization service
            builder.Register<BlueprintLocalizationService>(Lifetime.Singleton).AsImplementedInterfaces();
            
            // Register localization signals
            builder.DeclareSignal<LanguageChangingSignal>();
            builder.DeclareSignal<LanguageChangedSignal>();
            builder.DeclareSignal<BlueprintLocalizationCompletedSignal>();
        }
    }
}
