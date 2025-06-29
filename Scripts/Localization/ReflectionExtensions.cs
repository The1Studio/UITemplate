#if THEONE_LOCALIZATION
namespace TheOneStudio.UITemplate.Localization
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;

    /// <summary>
    /// Extension methods for reflection operations used in localization
    /// </summary>
    internal static class ReflectionExtensions
    {
        /// <summary>
        /// Check if a member has LocalizedFieldAttribute and is a string type
        /// </summary>
        public static bool HasValidLocalizedAttribute(this MemberInfo memberInfo)
        {
            if (memberInfo.GetCustomAttribute<LocalizedFieldAttribute>() == null)
                return false;

            var memberType = memberInfo.GetMemberType();
            
            if (memberType != typeof(string))
            {
                UnityEngine.Debug.LogError($"[LocalizedFieldAttribute] can only be applied to string type members. " +
                              $"Member '{memberInfo.Name}' in type '{memberInfo.DeclaringType?.Name}' is of type '{memberType?.Name}'.");
                return false;
            }

            return true;
        }

        /// <summary>
        /// Get the type of a member (field or property)
        /// </summary>
        public static Type GetMemberType(this MemberInfo memberInfo)
        {
            return memberInfo switch
            {
                FieldInfo field => field.FieldType,
                PropertyInfo property => property.PropertyType,
                _ => null
            };
        }

        /// <summary>
        /// Get value from a member (field or property)
        /// </summary>
        public static object GetMemberValue(this MemberInfo memberInfo, object target)
        {
            return memberInfo switch
            {
                FieldInfo field => field.GetValue(target),
                PropertyInfo property => property.GetValue(target),
                _ => null
            };
        }

        /// <summary>
        /// Set value to a member (field or property)
        /// </summary>
        public static void SetMemberValue(this MemberInfo memberInfo, object target, object value)
        {
            switch (memberInfo)
            {
                case FieldInfo field:
                    field.SetValue(target, value);
                    break;
                case PropertyInfo { CanWrite: true } property:
                    property.SetValue(target, value);
                    break;
            }
        }

        /// <summary>
        /// Check if a type is a generic Dictionary
        /// </summary>
        public static bool IsDictionary(this Type type)
        {
            return type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Dictionary<,>);
        }

        /// <summary>
        /// Check if a type is a generic List
        /// </summary>
        public static bool IsList(this Type type)
        {
            return type.IsGenericType && type.GetGenericTypeDefinition() == typeof(List<>);
        }

        /// <summary>
        /// Get all fields and properties of an object
        /// </summary>
        public static IEnumerable<MemberInfo> GetAllMembers(this Type type)
        {
            // Get fields
            var fields = type.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            foreach (var field in fields)
                yield return field;

            // Get properties
            var properties = type.GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            foreach (var property in properties)
                yield return property;
        }

        /// <summary>
        /// Get all public properties of an object
        /// </summary>
        public static IEnumerable<PropertyInfo> GetPublicProperties(this Type type)
        {
            return type.GetProperties(BindingFlags.Public | BindingFlags.Instance);
        }
    }
}
#endif
