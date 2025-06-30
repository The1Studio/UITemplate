namespace TheOneStudio.UITemplate.UITemplate.Localization.Signals
{
    using GameFoundation.Signals;

    /// <summary>
    /// Signal fired when language is changed
    /// </summary>
    public class LanguageChangedSignal
    {
        public string NewLanguage { get; set; }
    }

    public class LoadedLocalizationBlueprintsSignal
    {
    }

}