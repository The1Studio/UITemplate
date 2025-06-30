namespace TheOneStudio.UITemplate.UITemplate.Localization
{
    using GameFoundation.DI;
    using GameFoundation.Signals;
    using TheOneStudio.UITemplate.UITemplate.Localization.Signals;
    using VContainer;

    /// <summary>
    /// VContainer registration for localization services
    /// </summary>
    public static class UITemplateLocalizationVContainer
    {
        public static void RegisterLocalization(this IContainerBuilder builder)
        {
            builder.Register<UITemplateLocalizeBlueprint>(Lifetime.Singleton).AsImplementedInterfaces().AsSelf();
            builder.Register<UITemplateLocalizationManager>(Lifetime.Singleton).AsImplementedInterfaces().AsSelf();

            builder.DeclareSignal<LanguageChangedSignal>();
            builder.DeclareSignal<LoadedLocalizationBlueprintsSignal>();
        }
    }
}