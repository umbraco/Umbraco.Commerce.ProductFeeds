using System.Xml;

namespace Umbraco.Commerce.ProductFeeds.Core.Extensions
{
    internal static class XmlElementExtensions
    {
        /// <summary>
        /// Add a new child node at the bottom of the list of child nodes, of this node, only when the child node inner text has value.
        /// </summary>
        /// <param name="element"></param>
        /// <param name="childNodeName"></param>
        /// <param name="childNodeInnerText"></param>
        /// <param name="xmlNameSpace"></param>
        /// <returns>The added child node or null.</returns>
        public static XmlElement? AddChild(this XmlElement element, string childNodeName, string childNodeInnerText, string? xmlNameSpace = null)
        {
            if (!string.IsNullOrWhiteSpace(childNodeInnerText))
            {
                XmlElement childNode = element.OwnerDocument.CreateElement(childNodeName, xmlNameSpace);
                childNode.InnerText = childNodeInnerText;
                element.AppendChild(childNode);
                return childNode;
            }

            return null;
        }
    }
}
