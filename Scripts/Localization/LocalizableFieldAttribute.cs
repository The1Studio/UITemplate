namespace TheOneStudio.UITemplate.UITemplate.Localization
{
    using System;

    /// <summary>
    /// Attribute to mark fields that should be localized
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
    public class LocalizableFieldAttribute : Attribute
    {
        /// <summary>
        /// Optional localization key. If not provided, will use the field name or original value as key
        /// </summary>
        public string LocalizationKey { get; }

        /// <summary>
        /// Default constructor - uses field name as localization key
        /// </summary>
        public LocalizableFieldAttribute() { }

        /// <summary>
        /// Constructor with custom localization key
        /// </summary>
        /// <param name="localizationKey">Custom key for localization</param>
        public LocalizableFieldAttribute(string localizationKey)
        {
            this.LocalizationKey = localizationKey;
        }
    }
}
