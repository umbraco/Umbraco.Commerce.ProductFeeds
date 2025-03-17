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
            using (var writer = new XmlTextWriter(context.HttpContext.Response.Body, Encoding.UTF8))
            {
                writer.Formatting = Formatting;
                _document.WriteContentTo(writer);
#pragma warning disable CA1849 // The FlushAsync is not implemented
                writer.Flush();
#pragma warning restore CA1849 // The FlushAsync is not implemented
            }
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Exception generating the XML response");
        }
    }
}
