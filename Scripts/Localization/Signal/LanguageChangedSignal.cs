#if THEONE_LOCALIZATION
namespace TheOneStudio.UITemplate.Localization
{
    /// <summary>
    /// Signal fired when the language changes, notifying all blueprint objects to update their localized fields
    /// </summary>
    public class LanguageChangedSignal
    {
        public string NewLanguageCode { get; set; }
        
        public LanguageChangedSignal(string newLanguageCode)
        {
            this.NewLanguageCode = newLanguageCode;
        }
    }
}
#endif

