namespace TheOneStudio.UITemplate.UITemplate.Localization.Signals
{
    using GameFoundation.Signals;

    /// <summary>
    /// Signal fired when language is changed
    /// </summary>
    public class LanguageChangedSignal
    {
        public string NewLanguage { get; set; }
        public string OldLanguage { get; set; }
    }

    /// <summary>
    /// Signal fired when blueprint localization is completed
    /// </summary>
    public class BlueprintLocalizationCompletedSignal
    {
        public string Language { get; set; }
        public int LocalizedFieldsCount { get; set; }
        public int BlueprintCount { get; set; }
    }
}