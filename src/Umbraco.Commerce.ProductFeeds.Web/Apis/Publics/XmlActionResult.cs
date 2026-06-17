using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Umbraco.Commerce.ProductFeeds.Core.Common.Exceptions;

namespace Umbraco.Commerce.ProductFeeds.Web.Apis.Publics
{
    public sealed class XmlActionResult : IActionResult
    {
        private static readonly Serilog.ILogger _logger = Serilog.Log.Logger.ForContext(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private readonly XmlDocument _document;

        public Formatting Formatting { get; set; } = Formatting.None;
        public string MimeType { get; set; } = "text/xml";

        public XmlActionResult(XmlDocument document)
        {
            ArgumentNullException.ThrowIfNull(document);
            _document = document;
        }

        public async Task ExecuteResultAsync(ActionContext context)
        {
            context.HttpContext.Response.Clear();
            context.HttpContext.Response.ContentType = MimeType;

            try
            {
                // XmlDocument.WriteContentTo has no async variant and flushes synchronously.
                // Writing to a MemoryStream avoids the AllowSynchronousIO restriction on the
                // response body; we then copy to the response body asynchronously.
                using var memoryStream = new MemoryStream();

                var settings = new XmlWriterSettings
                {
                    Indent = Formatting == Formatting.Indented,
                    Encoding = Encoding.UTF8,
                };

                using (var xmlWriter = XmlWriter.Create(memoryStream, settings))
                {
                    _document.WriteContentTo(xmlWriter);
                }

                context.HttpContext.Response.ContentLength = memoryStream.Length;
                memoryStream.Seek(0, SeekOrigin.Begin);
                await memoryStream.CopyToAsync(context.HttpContext.Response.Body);
            }
            catch (UmbracoCommerceProductFeedsGeneralException ex)
            {
                _logger.Error(ex, "An error occurred while generating the XML response.");
            }
        }
    }
}
