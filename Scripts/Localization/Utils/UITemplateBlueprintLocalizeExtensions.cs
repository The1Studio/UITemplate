namespace TheOneStudio.UITemplate.UITemplate.Localization.Utils
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using BlueprintFlow.BlueprintReader;

    internal static class UITemplateBlueprintLocalizeExtensions
    {
        public static IEnumerable<PropertyInfo> GetLocalizableFields(Type type)
        {
            var properties = type.GetProperties(BindingFlags.Public | BindingFlags.Instance);

            return properties
                .Where(p => p.GetCustomAttribute<LocalizableFieldAttribute>() != null).ToList();

        }

        public static bool IsGenericBlueprintByRow(Type type)
        {
            // Check if type implements IBlueprintCollection
            return typeof(IBlueprintCollection).IsAssignableFrom(type);
        }

        public static bool IsGenericBlueprintByCol(Type type)
        {
            // Check if type implements IGenericBlueprintReader but NOT IBlueprintCollection
            return typeof(IGenericBlueprintReader).IsAssignableFrom(type) && !typeof(IBlueprintCollection).IsAssignableFrom(type);
        }
        public static bool IsDictionaryBlueprintByRow(Type type)
        {
            if (!typeof(IBlueprintCollection).IsAssignableFrom(type)) return false;
            if (!typeof(IDictionary).IsAssignableFrom(type)) return false;

            // Additional check: verify it has 2 generic arguments (TKey, TRecord pattern)
            if (type.IsGenericType)
            {
                var genericArgs = type.GetGenericArguments();
                return genericArgs.Length == 2;
            }

            // Check base types for generic Dictionary
            var current = type;
            while (current != null)
            {
                if (current.IsGenericType)
                {
                    var genericTypeDef = current.GetGenericTypeDefinition();
                    if (genericTypeDef == typeof(Dictionary<,>))
                    {
                        return true;
                    }
                }
                current = current.BaseType;
            }

            return false;
        }

        public static bool IsListBlueprintByRow(Type type)
        {
            if (!typeof(IBlueprintCollection).IsAssignableFrom(type)) return false;
            if (!typeof(IList).IsAssignableFrom(type) || typeof(IDictionary).IsAssignableFrom(type)) return false;

            // Additional check: verify it has 1 generic argument (TRecord pattern)
            if (type.IsGenericType)
            {
                var genericArgs = type.GetGenericArguments();
                return genericArgs.Length == 1;
            }

            // Check base types for generic List
            var current = type;
            while (current != null)
            {
                if (current.IsGenericType)
                {
                    var genericTypeDef = current.GetGenericTypeDefinition();
                    if (genericTypeDef == typeof(List<>))
                    {
                        return true;
                    }
                }
                current = current.BaseType;
            }

            return false;
        }
    }
}