using System;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Umbraco.Commerce.ProductFeeds.Apis.Public;

public sealed class XmlActionResult : IActionResult
{
    private static readonly Serilog.ILogger _logger = Serilog.Log.Logger.ForContext(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

    private readonly XmlDocument _document;

    public Formatting Formatting { get; set; }
    public string MimeType { get; set; }

    public XmlActionResult(XmlDocument document)
    {
        if (document == null)
            throw new ArgumentNullException("document");

        _document = document;

        // Default values
        MimeType = "text/xml";
        Formatting = Formatting.None;
    }

    public async Task ExecuteResultAsync(ActionContext context)
    {
        context.HttpContext.Response.Clear();
        context.HttpContext.Response.ContentType = MimeType;

        try
        {
            using (var writer = new XmlTextWriter(context.HttpContext.Response.Body, Encoding.UTF8))
            {
                writer.Formatting = Formatting;
                _document.WriteContentTo(writer);
                writer.Flush();
            }
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Exception generating the XML response");
        }
    }
}
