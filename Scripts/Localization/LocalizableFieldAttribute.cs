namespace TheOneStudio.UITemplate.UITemplate.Localization
{
    using System;

    /// <summary>
    /// Attribute to mark blueprint fields for automatic localization.
    /// Fields marked with this attribute will be automatically updated when language changes.
    /// Must be Properties, have set, type is string.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
    public class LocalizableFieldAttribute : Attribute
    {

    }
}