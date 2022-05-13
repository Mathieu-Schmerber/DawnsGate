using System;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Nawlian.Lib.Extensions
{
    public static class ReflectionExtensions
    {
        /// <summary>
        /// Get a property's attribute
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="property"></param>
        /// <param name="isRequired"></param>
        /// <returns></returns>
        public static T GetAttribute<T>(this PropertyInfo property, bool isRequired) where T : Attribute
        {
            var attribute = property.GetCustomAttributes(typeof(T), false).SingleOrDefault();

            if (attribute == null && isRequired)
            {
                throw new ArgumentException(
                    string.Format(
                        CultureInfo.InvariantCulture,
                        "The {0} attribute must be defined on property {1}",
                        typeof(T).Name,
                        property.Name));
            }

            return (T)attribute;
        }

        /// <summary>
        /// Get a member's attribute
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="member"></param>
        /// <param name="isRequired"></param>
        /// <returns></returns>
        public static T GetAttribute<T>(this MemberInfo member, bool isRequired) where T : Attribute
        {
            var attribute = member.GetCustomAttributes(typeof(T), false).SingleOrDefault();

            if (attribute == null && isRequired)
            {
                throw new ArgumentException(
                    string.Format(
                        CultureInfo.InvariantCulture,
                        "The {0} attribute must be defined on member {1}",
                        typeof(T).Name,
                        member.Name));
            }

            return (T)attribute;
        }

        /// <summary>
        /// Get the DisplayName attribute value of a property
        /// </summary>
        /// <param name="type"></param>
        /// <param name="propertyName"></param>
        /// <returns></returns>
        public static string GetDisplayNameByPropertyName(this Type type, string propertyName)
        {
            PropertyInfo property = type.GetProperty(propertyName);
            DisplayNameAttribute display = property?.GetCustomAttribute(typeof(DisplayNameAttribute)) as DisplayNameAttribute;

            return display == null ? null : display.DisplayName;
        }
    }
}