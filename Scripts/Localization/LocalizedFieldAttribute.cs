#if THEONE_LOCALIZATION
namespace TheOneStudio.UITemplate.Localization
{
    using System;

    /// <summary>
    /// Attribute to mark string fields in blueprint records that should be automatically localized.
    /// The field value will be treated as a localization key.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, Inherited = true)]
    public class LocalizedFieldAttribute : Attribute
    {
        // No parameters needed - the field value itself is the localization key
    }
}
#endif
