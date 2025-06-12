using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

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
                // Create a StreamWriter over the response body with async encoding support
                await using var streamWriter = new StreamWriter(context.HttpContext.Response.Body, Encoding.UTF8, leaveOpen: true);

                // Use XmlWriter with async settings
                var settings = new XmlWriterSettings
                {
                    Async = true,
                    Indent = Formatting == Formatting.Indented,
                };

                await using var xmlWriter = XmlWriter.Create(streamWriter, settings);

                // Write the document content
                _document.WriteContentTo(xmlWriter);

                // Async flush
                await xmlWriter.FlushAsync();
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Exception generating the XML response");
            }
        }
    }
}
