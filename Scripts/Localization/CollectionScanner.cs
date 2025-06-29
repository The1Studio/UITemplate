#if THEONE_LOCALIZATION
namespace TheOneStudio.UITemplate.Localization
{
    using System.Collections;
    using System.Collections.Generic;
    using System.Reflection;
    using BlueprintFlow.BlueprintReader;

    /// <summary>
    /// Handles scanning of collections and nested objects for localized fields
    /// </summary>
    internal static class CollectionScanner
    {
        /// <summary>
        /// Scan a Dictionary collection for localized fields in its values
        /// </summary>
        public static List<LocalizedFieldInfo> ScanDictionary(IDictionary dictionary)
        {
            var localizedFields = new List<LocalizedFieldInfo>();
            
            if (dictionary == null) return localizedFields;

            foreach (var value in dictionary.Values)
            {
                if (value != null)
                {
                    localizedFields.AddRange(ScanObjectForLocalizedFields(value));
                    localizedFields.AddRange(ScanObjectForNestedCollections(value));
                }
            }

            return localizedFields;
        }

        /// <summary>
        /// Scan a List collection for localized fields in its items
        /// </summary>
        public static List<LocalizedFieldInfo> ScanList(IEnumerable list)
        {
            var localizedFields = new List<LocalizedFieldInfo>();
            
            if (list == null) return localizedFields;

            foreach (var item in list)
            {
                if (item != null)
                {
                    localizedFields.AddRange(ScanObjectForLocalizedFields(item));
                    localizedFields.AddRange(ScanObjectForNestedCollections(item));
                }
            }

            return localizedFields;
        }

        /// <summary>
        /// Scan an object for direct localized fields (fields/properties with LocalizedFieldAttribute)
        /// </summary>
        public static List<LocalizedFieldInfo> ScanObjectForLocalizedFields(object obj)
        {
            var localizedFields = new List<LocalizedFieldInfo>();
            var objectType = obj.GetType();

            foreach (var member in objectType.GetAllMembers())
            {
                if (member.HasValidLocalizedAttribute())
                {
                    localizedFields.Add(new LocalizedFieldInfo
                    {
                        MemberInfo = member,
                        IsProperty = member is PropertyInfo,
                        TargetObject = obj
                    });
                }
            }

            return localizedFields;
        }

        /// <summary>
        /// Scan an object for nested collections that might contain localized fields
        /// </summary>
        public static List<LocalizedFieldInfo> ScanObjectForNestedCollections(object obj)
        {
            var localizedFields = new List<LocalizedFieldInfo>();
            var objectType = obj.GetType();

            foreach (var member in objectType.GetAllMembers())
            {
                var memberValue = member.GetMemberValue(obj);
                if (memberValue == null) continue;

                var memberType = member.GetMemberType();

                // Check for nested IBlueprintCollection
                if (IsBlueprintCollection(memberType))
                {
                    if (memberType.IsDictionary())
                    {
                        localizedFields.AddRange(ScanDictionary(memberValue as IDictionary));
                    }
                    else if (memberType.IsList())
                    {
                        localizedFields.AddRange(ScanList(memberValue as IEnumerable));
                    }
                }
                // Check for nested objects using blueprint flow logic
                else if (IsBlueprintNested(member))
                {
                    localizedFields.AddRange(ScanObjectForLocalizedFields(memberValue));
                    localizedFields.AddRange(ScanObjectForNestedCollections(memberValue));
                }
            }

            return localizedFields;
        }

        /// <summary>
        /// Scan a blueprint reader for all localized fields in its collections and properties
        /// </summary>
        public static List<LocalizedFieldInfo> ScanBlueprintReader(IGenericBlueprintReader blueprintReader)
        {
            var localizedFields = new List<LocalizedFieldInfo>();
            var readerType = blueprintReader.GetType();

            foreach (var property in readerType.GetPublicProperties())
            {
                var propertyValue = property.GetValue(blueprintReader);
                if (propertyValue == null) continue;

                var propertyType = property.PropertyType;

                // Handle Dictionary<TKey, TValue> (for ByRow readers)
                if (propertyType.IsDictionary())
                {
                    localizedFields.AddRange(ScanDictionary(propertyValue as IDictionary));
                }
                // Handle List<T> (for potential list properties)
                else if (propertyType.IsList())
                {
                    localizedFields.AddRange(ScanList(propertyValue as IEnumerable));
                }
                // Check property of reader itself (for ByCol readers)
                else if (property.HasValidLocalizedAttribute())
                {
                    localizedFields.Add(new LocalizedFieldInfo
                    {
                        MemberInfo = property,
                        IsProperty = true,
                        TargetObject = blueprintReader
                    });
                }
            }

            return localizedFields;
        }

        /// <summary>
        /// Check if a type is a blueprint collection (same logic as in GenericBlueprintReaderByRow)
        /// </summary>
        private static bool IsBlueprintCollection(System.Type type)
        {
            return (type.IsGenericType || type.BaseType is { IsGenericType: true }) && typeof(IBlueprintCollection).IsAssignableFrom(type);
        }

        /// <summary>
        /// Check if a member is a blueprint nested object (same logic as in GenericBlueprintReaderByRow)
        /// </summary>
        private static bool IsBlueprintNested(MemberInfo memberInfo)
        {
            return memberInfo.IsDefined(typeof(NestedBlueprintAttribute), false) &&
                   (memberInfo.GetMemberType().IsClass || memberInfo.GetMemberType().IsValueType);
        }
    }

    /// <summary>
    /// Information about a localized field
    /// </summary>
    internal class LocalizedFieldInfo
    {
        public MemberInfo MemberInfo { get; set; }
        public bool IsProperty { get; set; }
        public object TargetObject { get; set; }
    }
}
#endif
