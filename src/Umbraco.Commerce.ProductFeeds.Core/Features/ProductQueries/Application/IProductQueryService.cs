using System.Collections.Generic;
using Umbraco.Cms.Core.Models.PublishedContent;

namespace Umbraco.Commerce.ProductFeeds.Core.ProductQueries.Application
{
    public interface IProductQueryService
    {
        ICollection<IPublishedContent> GetPublishedProducts(GetPublishedProductsParams parameters);
    }
}
