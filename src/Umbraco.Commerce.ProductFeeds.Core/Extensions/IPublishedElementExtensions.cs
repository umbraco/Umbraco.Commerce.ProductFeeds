using System.Reflection;
using Umbraco.Cms.Core.Models.PublishedContent;

namespace Umbraco.Commerce.ProductFeeds.Extensions
{
    internal static class IPublishedElementExtensions
    {
        private const BindingFlags DefaultReflectionPropertyLookup = BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public;

        /// <summary>
        /// Get either built-in property or custom properties by property alias. Fallback to the value of . Property alias is case-insensitive.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="content"></param>
        /// <param name="propertyAlias">Case-insensitive property alias.</param>
        /// <param name="fallbackElement">Store fallback properties.</param>
        /// <returns></returns>
        internal static T? GetPropertyValue<T>(this IPublishedElement content, string propertyAlias, IPublishedElement? fallbackElement)
        {
            ArgumentNullException.ThrowIfNull(content);

            T? propValue = content.Value<T>(propertyAlias);
            if (propValue != null)
            {
                return propValue;
            }

            // try getting fallback value
            if (fallbackElement != null)
            {
                T? fallbackValue = fallbackElement.GetPropertyValue<T>(propertyAlias, null);
                if (fallbackValue != null)
                {
                    return fallbackValue;
                }
            }

            // fallback to umbraco built-in property
            PropertyInfo? propertyInfo = content.GetType().GetProperty(propertyAlias, DefaultReflectionPropertyLookup | BindingFlags.IgnoreCase);
            if (propertyInfo != null)
            {
                object? propertyValue = propertyInfo.GetValue(content);
                return (T?)propertyValue;
            }

            return default;
        }
    }
}
