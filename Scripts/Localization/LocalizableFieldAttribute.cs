namespace TheOneStudio.UITemplate.UITemplate.Localization
{
    using System;

    /// <summary>
    /// Attribute to mark blueprint fields for automatic localization.
    /// Fields marked with this attribute will be automatically updated when language changes.
    /// The localization key will be derived from the property name.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
    public class LocalizableFieldAttribute : Attribute
    {
        // Simple attribute without any parameters
        // The property/field name will be used as the localization key
    }
}