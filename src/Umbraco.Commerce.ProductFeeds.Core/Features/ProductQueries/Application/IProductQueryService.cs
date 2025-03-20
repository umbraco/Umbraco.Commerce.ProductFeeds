using Umbraco.Cms.Core.Models.PublishedContent;

namespace Umbraco.Commerce.ProductFeeds.Core.Features.ProductQueries.Application
{
    public interface IProductQueryService
    {
        /// <summary>
        /// Get published products filtered by input parameters.
        /// </summary>
        /// <param name="parameters"></param>
        /// <exception cref="ContentNotFoundException"></exception>
        /// <returns></returns>
        ICollection<IPublishedContent> GetPublishedProducts(GetPublishedProductsParams parameters);
    }
}
