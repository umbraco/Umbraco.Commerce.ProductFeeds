using System.Reflection;
using Umbraco.Cms.Core.Models.PublishedContent;

namespace Umbraco.Commerce.ProductFeeds.Core.Commons.Extensions
{
    internal static class IPublishedContentExtensions
    {
        private const BindingFlags DefaultReflectionPropertyLookup = BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public;

        /// <summary>
        /// Get either built-in property or custom properties by property alias. Property alias is case-insensitive.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="content"></param>
        /// <param name="propertyAlias">Case-insensitive property alias.</param>
        /// <returns></returns>
        internal static T? GetPropertyValue<T>(this IPublishedContent content, string propertyAlias)
        {
            if (content.HasProperty(propertyAlias))
            {
                return content.Value<T>(propertyAlias);
            }

            PropertyInfo? propertyInfo = typeof(IPublishedContent).GetProperty(propertyAlias, DefaultReflectionPropertyLookup | BindingFlags.IgnoreCase);
            if (propertyInfo != null)
            {
                object? propertyValue = propertyInfo.GetValue(content);
                return (T?)propertyValue;
            }

            return default;
        }
    }
}
